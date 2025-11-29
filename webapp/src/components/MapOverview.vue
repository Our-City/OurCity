<!-- Generative AI was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax, and error handling.
  Map component for displaying all posts on a map with interactive markers -->
<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import { loadGoogleMaps } from "@/utils/googleMapsLoader";
import { getMediaByPostId } from "@/api/mediaService";
import type { Post } from "@/models/post";
import type { Media } from "@/models/media";
import { removePostalCode } from "@/utils/locationFormatter";

interface Props {
  posts: Post[];
  height?: string;
}

const props = withDefaults(defineProps<Props>(), {
  height: "600px",
});

const router = useRouter();
const mapContainer = ref<HTMLDivElement | null>(null);
let map: google.maps.Map | null = null;
let heatmap: google.maps.visualization.HeatmapLayer | null = null; // Add heatmap layer
const markers = ref<Map<string, google.maps.marker.AdvancedMarkerElement>>(new Map());
const infoWindows = ref<Map<string, google.maps.InfoWindow>>(new Map());
const postMedia = ref<Map<string, Media[]>>(new Map());

const isLoading = ref(true);
const error = ref<string | null>(null);
const showHeatmap = ref(false); // Toggle state for heatmap
const showMarkers = ref(true); // Toggle state for markers

const DEFAULT_CENTER = { lat: 49.8951, lng: -97.1384 };

// Fetch media for all posts with locations
async function fetchPostsMedia(posts: Post[]) {
  const postsWithLocation = posts.filter(
    (post) => post.latitude && post.longitude && !post.isDeleted,
  );

  const mediaPromises = postsWithLocation.map(async (post) => {
    try {
      const media = await getMediaByPostId(post.id);
      return { postId: post.id, media };
    } catch (err) {
      console.error(`Failed to fetch media for post ${post.id}:`, err);
      return { postId: post.id, media: [] };
    }
  });

  const results = await Promise.all(mediaPromises);

  results.forEach(({ postId, media }) => {
    postMedia.value.set(postId, media);
  });
}

async function initMap() {
  try {
    isLoading.value = true;
    error.value = null;

    if (!mapContainer.value) {
      throw new Error("Map container not found");
    }

    await loadGoogleMaps();
    await new Promise((resolve) => setTimeout(resolve, 100));

    map = new google.maps.Map(mapContainer.value, {
      center: DEFAULT_CENTER,
      zoom: 12,
      mapTypeControl: true,
      streetViewControl: false,
      fullscreenControl: true,
      mapId: "OURCITY_MAP_OVERVIEW",
    });

    // Fetch media and add markers
    await fetchPostsMedia(props.posts);
    addMarkers();
    initHeatmap(); // Initialize heatmap

    isLoading.value = false;
  } catch (err) {
    console.error("Error loading Google Maps:", err);
    error.value = "Failed to load map.";
    isLoading.value = false;
  }
}

// Initialize heatmap layer
function initHeatmap() {
  if (!map) return;

  const postsWithLocation = props.posts.filter(
    (post) => post.latitude && post.longitude && !post.isDeleted,
  );

  if (postsWithLocation.length === 0) {
    return;
  }

  // Create weighted data points (more posts at same location = higher weight)
  const locationCounts = new Map<string, { location: google.maps.LatLng; count: number }>();

  postsWithLocation.forEach((post) => {
    const key = `${post.latitude},${post.longitude}`;
    const existing = locationCounts.get(key);

    if (existing) {
      existing.count++;
    } else {
      locationCounts.set(key, {
        location: new google.maps.LatLng(post.latitude!, post.longitude!),
        count: 1,
      });
    }
  });

  // Convert to weighted heatmap data
  const heatmapData: google.maps.visualization.WeightedLocation[] = Array.from(
    locationCounts.values(),
  ).map((item) => ({
    location: item.location,
    weight: item.count, // Higher count = hotter spot
  }));

  // Create heatmap layer
  heatmap = new google.maps.visualization.HeatmapLayer({
    data: heatmapData,
    map: showHeatmap.value ? map : null, // Only show if toggle is on
    radius: 30, // Size of each heat point
    opacity: 0.6, // Transparency
    gradient: [
      "rgba(0, 255, 255, 0)",
      "rgba(0, 255, 255, 1)",
      "rgba(0, 191, 255, 1)",
      "rgba(0, 127, 255, 1)",
      "rgba(0, 63, 255, 1)",
      "rgba(0, 0, 255, 1)",
      "rgba(0, 0, 223, 1)",
      "rgba(0, 0, 191, 1)",
      "rgba(0, 0, 159, 1)",
      "rgba(0, 0, 127, 1)",
      "rgba(63, 0, 91, 1)",
      "rgba(127, 0, 63, 1)",
      "rgba(191, 0, 31, 1)",
      "rgba(255, 0, 0, 1)",
    ],
  });
}

// Toggle heatmap visibility
function toggleHeatmap() {
  showHeatmap.value = !showHeatmap.value;

  if (heatmap) {
    heatmap.setMap(showHeatmap.value ? map : null);
  }
}

// Toggle markers visibility
function toggleMarkers() {
  showMarkers.value = !showMarkers.value;

  markers.value.forEach((marker) => {
    marker.map = showMarkers.value ? map : null;
  });
}

function addMarkers() {
  if (!map) return;

  clearMarkers();

  const postsWithLocation = props.posts.filter(
    (post) => post.latitude && post.longitude && !post.isDeleted,
  );

  if (postsWithLocation.length === 0) {
    return;
  }

  const bounds = new google.maps.LatLngBounds();

  postsWithLocation.forEach((post) => {
    const position = { lat: post.latitude!, lng: post.longitude! };

    const marker = new google.maps.marker.AdvancedMarkerElement({
      map: showMarkers.value ? map : null,
      position,
      title: post.title,
    });

    const media = postMedia.value.get(post.id) || [];
    const infoWindowContent = createInfoWindowContent(post, media);
    const infoWindow = new google.maps.InfoWindow({
      content: infoWindowContent,
    });

    marker.addListener("click", () => {
      infoWindows.value.forEach((iw) => iw.close());
      infoWindow.open(map, marker);
    });

    markers.value.set(post.id, marker);
    infoWindows.value.set(post.id, infoWindow);
    bounds.extend(position);
  });

  if (postsWithLocation.length > 0) {
    map.fitBounds(bounds);

    google.maps.event.addListenerOnce(map, "idle", () => {
      if (map && map.getZoom()! > 15) {
        map.setZoom(15);
      }
    });
  }
}

function createInfoWindowContent(post: Post, media: Media[]): HTMLDivElement {
  const content = document.createElement("div");
  content.className = "map-info-window";
  content.style.cssText = `
    max-width: 280px;
    padding: 12px;
    cursor: pointer;
  `;

  if (media.length > 0) {
    const imageContainer = document.createElement("div");
    imageContainer.style.cssText = `
      width: 100%;
      height: 140px;
      margin-bottom: 12px;
      border-radius: 8px;
      overflow: hidden;
      background: #f3f4f6;
    `;

    const image = document.createElement("img");
    image.src = media[0].url;
    image.alt = post.title;
    image.style.cssText = `
      width: 100%;
      height: 100%;
      object-fit: cover;
    `;

    image.onerror = () => {
      imageContainer.style.display = "none";
    };

    imageContainer.appendChild(image);
    content.appendChild(imageContainer);
  }

  const title = document.createElement("h3");
  title.textContent = post.title;
  title.style.cssText = `
    margin: 0 0 8px 0;
    font-size: 16px;
    font-weight: 600;
    color: #1f2937;
    line-height: 1.3;
  `;

  const description = document.createElement("p");
  description.textContent =
    post.description.length > 80 ? post.description.substring(0, 80) + "..." : post.description;
  description.style.cssText = `
    margin: 0 0 8px 0;
    font-size: 13px;
    color: #6b7280;
    line-height: 1.4;
  `;

  const location = document.createElement("p");
  const formattedLocation = removePostalCode(post.location) || "Winnipeg";
  location.innerHTML = `<i class="pi pi-map-marker"></i> ${formattedLocation}`;
  location.style.cssText = `
    margin: 0 0 8px 0;
    font-size: 12px;
    color: #9ca3af;
    display: flex;
    align-items: center;
    gap: 4px;
  `;

  const stats = document.createElement("div");
  stats.style.cssText = `
    display: flex;
    gap: 12px;
    margin-bottom: 10px;
    font-size: 12px;
    color: #6b7280;
  `;
  stats.innerHTML = `
    <span style="display: flex; align-items: center; gap: 4px;">
      <i class="pi pi-arrow-up"></i> ${post.voteCount}
    </span>
    <span style="display: flex; align-items: center; gap: 4px;">
      <i class="pi pi-comments"></i> ${post.commentCount}
    </span>
  `;

  const button = document.createElement("button");
  button.textContent = "View Post";
  button.style.cssText = `
    width: 100%;
    padding: 8px 12px;
    background: #3b82f6;
    color: white;
    border: none;
    border-radius: 6px;
    font-size: 13px;
    font-weight: 500;
    cursor: pointer;
    transition: background 0.2s;
  `;
  button.onmouseover = () => {
    button.style.background = "#2563eb";
  };
  button.onmouseout = () => {
    button.style.background = "#3b82f6";
  };
  button.onclick = (e) => {
    e.stopPropagation();
    router.push(`/posts/${post.id}`);
  };

  content.appendChild(title);
  content.appendChild(description);
  content.appendChild(location);
  content.appendChild(stats);
  content.appendChild(button);

  return content;
}

function clearMarkers() {
  markers.value.forEach((marker) => {
    marker.map = null;
  });
  markers.value.clear();

  infoWindows.value.forEach((iw) => iw.close());
  infoWindows.value.clear();
}

watch(
  () => props.posts,
  async (newPosts) => {
    if (map) {
      await fetchPostsMedia(newPosts);
      addMarkers();

      // Recreate heatmap with new data
      if (heatmap) {
        heatmap.setMap(null);
      }
      initHeatmap();
    }
  },
  { deep: true },
);

onMounted(initMap);
</script>

<template>
  <div class="map-overview-container">
    <div v-if="isLoading" class="loading-state">
      <i class="pi pi-spin pi-spinner"></i>
      <p>Loading map...</p>
    </div>

    <div v-else-if="error" class="error-state">
      <i class="pi pi-exclamation-triangle"></i>
      <p>{{ error }}</p>
    </div>

    <div
      v-show="!isLoading && !error"
      ref="mapContainer"
      class="map-container"
      :style="{ height: props.height }"
    ></div>

    <!-- Toggle Controls - positioned below map -->
    <div v-if="!isLoading && !error" class="map-controls">
      <button
        class="control-button"
        :class="{ active: showMarkers }"
        @click="toggleMarkers"
        title="Toggle Markers"
      >
        <i class="pi pi-map-marker"></i>
        Markers
      </button>
      <button
        class="control-button"
        :class="{ active: showHeatmap }"
        @click="toggleHeatmap"
        title="Toggle Heatmap"
      >
        <i class="pi pi-chart-bar"></i>
        Heatmap
      </button>
    </div>

    <div v-if="!isLoading && !error && props.posts.length === 0" class="empty-state">
      <i class="pi pi-map"></i>
      <p>No posts with locations to display</p>
    </div>
  </div>
</template>

<style scoped>
.map-overview-container {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.map-controls {
  display: flex;
  justify-content: center;
  gap: 8px;
  background: white;
  padding: 8px;
  border-radius: 8px;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
  width: 100%;
}

.control-button {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  background: white;
  border: 2px solid #e5e7eb;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 500;
  color: #6b7280;
  cursor: pointer;
  transition: all 0.2s;
}

.control-button:hover {
  background: #f9fafb;
  border-color: #3b82f6;
  color: #3b82f6;
}

.control-button.active {
  background: #3b82f6;
  border-color: #3b82f6;
  color: white;
}

.control-button i {
  font-size: 14px;
}

.map-container {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  flex: 1;
}

.loading-state,
.error-state,
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 2rem;
  color: var(--secondary-text-color);
}

.loading-state i,
.error-state i,
.empty-state i {
  font-size: 2rem;
}

.error-state {
  color: #ef4444;
}
</style>
