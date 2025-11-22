<!-- filepath: webapp/src/components/MapOverview.vue -->
<!-- Map component for displaying all posts on a map with interactive markers -->
<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import { loadGoogleMaps } from "@/utils/googleMapsLoader";
import type { Post } from "@/models/post";

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
const markers = ref<Map<string, google.maps.marker.AdvancedMarkerElement>>(new Map());
const infoWindows = ref<Map<string, google.maps.InfoWindow>>(new Map());

const isLoading = ref(true);
const error = ref<string | null>(null);

// Default center (Winnipeg)
const DEFAULT_CENTER = { lat: 49.8951, lng: -97.1384 };

// Initialize Google Maps
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

    // Add markers for posts with locations
    addMarkers();

    isLoading.value = false;
  } catch (err) {
    console.error("Error loading Google Maps:", err);
    error.value = "Failed to load map.";
    isLoading.value = false;
  }
}

// Add markers for all posts with location data
function addMarkers() {
  if (!map) return;

  // Clear existing markers
  clearMarkers();

  const postsWithLocation = props.posts.filter(
    (post) => post.latitude && post.longitude && !post.isDeleted,
  );

  if (postsWithLocation.length === 0) {
    return;
  }

  // Create bounds to fit all markers
  const bounds = new google.maps.LatLngBounds();

  postsWithLocation.forEach((post) => {
    const position = { lat: post.latitude!, lng: post.longitude! };

    // Create marker
    const marker = new google.maps.marker.AdvancedMarkerElement({
      map,
      position,
      title: post.title,
    });

    // Create info window content
    const infoWindowContent = createInfoWindowContent(post);
    const infoWindow = new google.maps.InfoWindow({
      content: infoWindowContent,
    });

    // Add click listener to marker
    marker.addListener("click", () => {
      // Close all other info windows
      infoWindows.value.forEach((iw) => iw.close());
      // Open this info window
      infoWindow.open(map, marker);
    });

    markers.value.set(post.id, marker);
    infoWindows.value.set(post.id, infoWindow);
    bounds.extend(position);
  });

  // Fit map to show all markers
  if (postsWithLocation.length > 0) {
    map.fitBounds(bounds);
    
    // Ensure zoom doesn't get too close if there's only one marker
    const listener = google.maps.event.addListenerOnce(map, "idle", () => {
      if (map && map.getZoom()! > 15) {
        map.setZoom(15);
      }
    });
  }
}

// Create info window HTML content
function createInfoWindowContent(post: Post): HTMLDivElement {
  const content = document.createElement("div");
  content.className = "map-info-window";
  content.style.cssText = `
    max-width: 250px;
    padding: 12px;
    cursor: pointer;
  `;

  const title = document.createElement("h3");
  title.textContent = post.title;
  title.style.cssText = `
    margin: 0 0 8px 0;
    font-size: 16px;
    font-weight: 600;
    color: #1f2937;
  `;

  const description = document.createElement("p");
  description.textContent =
    post.description.length > 100
      ? post.description.substring(0, 100) + "..."
      : post.description;
  description.style.cssText = `
    margin: 0 0 8px 0;
    font-size: 14px;
    color: #6b7280;
  `;

  const location = document.createElement("p");
  location.innerHTML = `<i class="pi pi-map-marker"></i> ${post.location || "Winnipeg"}`;
  location.style.cssText = `
    margin: 0 0 8px 0;
    font-size: 12px;
    color: #9ca3af;
  `;

  const stats = document.createElement("div");
  stats.style.cssText = `
    display: flex;
    gap: 12px;
    margin-bottom: 8px;
    font-size: 12px;
    color: #6b7280;
  `;
  stats.innerHTML = `
    <span><i class="pi pi-arrow-up"></i> ${post.voteCount}</span>
    <span><i class="pi pi-comments"></i> ${post.commentCount}</span>
  `;

  const button = document.createElement("button");
  button.textContent = "View Post";
  button.style.cssText = `
    width: 100%;
    padding: 8px;
    background: #3b82f6;
    color: white;
    border: none;
    border-radius: 6px;
    font-size: 14px;
    font-weight: 500;
    cursor: pointer;
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

// Clear all markers from map
function clearMarkers() {
  markers.value.forEach((marker) => {
    marker.map = null;
  });
  markers.value.clear();

  infoWindows.value.forEach((iw) => iw.close());
  infoWindows.value.clear();
}

// Watch for changes in posts
watch(
  () => props.posts,
  () => {
    if (map) {
      addMarkers();
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
  position: relative;
}

.map-container {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
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