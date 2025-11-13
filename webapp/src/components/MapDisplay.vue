<!-- Read-only Google Maps component for displaying a single location
  Shows a marker at a specific location without interaction
  Generative AI - CoPilot was used to assist in the creation of this file.
    CoPilot was asked to provide help with CSS styling and for help with syntax.
    It also assisted with error handling -->
<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { loadGoogleMaps } from "@/utils/googleMapsLoader";

interface Props {
  latitude?: number;
  longitude?: number;
  locationName?: string;
  height?: string;
  zoom?: number;
}

const props = withDefaults(defineProps<Props>(), {
  height: "400px",
  zoom: 15,
});

// Refs for map and marker
const mapContainer = ref<HTMLDivElement | null>(null);
let map: google.maps.Map | null = null;
let marker: google.maps.marker.AdvancedMarkerElement | null = null;

const isLoading = ref(true);
const error = ref<string | null>(null);
const hasLocation = ref(false);

// Default center (Winnipeg) - used when no location is provided
const DEFAULT_CENTER = { lat: 49.8951, lng: -97.1384 };

// Initialize Google Maps
async function initMap() {
  try {
    isLoading.value = true;
    error.value = null;

    if (!mapContainer.value) {
      throw new Error("Map container not found");
    }

    // Load Google Maps using shared utility
    await loadGoogleMaps();

    // Small delay to ensure all libraries are ready
    await new Promise((resolve) => setTimeout(resolve, 100));

    // Check if we have a valid location
    hasLocation.value = !!(props.latitude && props.longitude);

    // Determine map center
    const center = hasLocation.value
      ? { lat: props.latitude!, lng: props.longitude! }
      : DEFAULT_CENTER;

    // Create map instance
    map = new google.maps.Map(mapContainer.value, {
      center,
      zoom: hasLocation.value ? props.zoom : 12,
      mapTypeControl: true,
      streetViewControl: false,
      fullscreenControl: true,
      mapId: "OURCITY_MAP_DISPLAY",
      // Disable interactions for read-only display
      disableDefaultUI: false,
      draggable: true, // Allow panning to explore
      scrollwheel: true, // Allow zoom
      disableDoubleClickZoom: false,
    });

    // If we have a location, place a marker
    if (hasLocation.value) {
      placeMarker({ lat: props.latitude!, lng: props.longitude! });
    }

    isLoading.value = false;
  } catch (err) {
    console.error("Error loading Google Maps:", err);
    error.value = "Failed to load map.";
    isLoading.value = false;
  }
}

// Place marker on map
function placeMarker(position: { lat: number; lng: number }) {
  if (!map) return;

  // Create Advanced Marker
  marker = new google.maps.marker.AdvancedMarkerElement({
    map,
    position,
    title: props.locationName || "Post Location",
  });
}

// Watch for changes to location props
watch(
  () => [props.latitude, props.longitude],
  ([newLat, newLng]) => {
    if (newLat && newLng && map) {
      hasLocation.value = true;
      const position = { lat: newLat as number, lng: newLng as number };

      // Update or create marker
      if (marker) {
        marker.position = position;
      } else {
        placeMarker(position);
      }

      // Re-center map on new location
      map.setCenter(position);
      map.setZoom(props.zoom);
    } else {
      hasLocation.value = false;
      // Remove marker if it exists
      if (marker) {
        marker.map = null;
        marker = null;
      }
    }
  },
);

// Initialize map on mount
onMounted(() => {
  initMap();
});
</script>

<template>
  <div class="map-display">
    <div v-if="isLoading" class="map-loading">
      <i class="pi pi-spin pi-spinner"></i>
      <p>Loading map...</p>
    </div>

    <div v-else-if="error" class="map-error">
      <i class="pi pi-exclamation-triangle"></i>
      <p>{{ error }}</p>
    </div>

    <div v-else-if="!hasLocation" class="map-no-location">
      <i class="pi pi-map-marker"></i>
      <p>No location specified</p>
    </div>

    <div
      v-show="!isLoading && !error && hasLocation"
      ref="mapContainer"
      class="map-container"
      :style="{ height: props.height }"
    ></div>

    <div v-if="hasLocation && locationName && !isLoading" class="map-info">
      <div class="location-details">
        <i class="pi pi-map-marker"></i>
        <span class="location-name">{{ locationName }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.map-display {
  width: 100%;
  border: 1px solid var(--border-color);
  border-radius: 0.5rem;
  overflow: hidden;
  background: var(--primary-background-color);
}

.map-container {
  width: 100%;
  min-height: 300px;
}

.map-loading,
.map-error,
.map-no-location {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 400px;
  gap: 1rem;
  color: var(--secondary-text-color);
}

.map-loading i {
  font-size: 3rem;
}

.map-error {
  color: var(--error-color, #dc3545);
}

.map-error i {
  font-size: 3rem;
}

.map-no-location {
  background: var(--neutral-color);
}

.map-no-location i {
  font-size: 3rem;
  color: var(--secondary-text-color);
}

.map-info {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 1rem;
  background: var(--neutral-color);
  border-top: 1px solid var(--border-color);
}

.location-details {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.location-details i {
  color: var(--primary-color, #3b82f6);
}

.location-name {
  font-size: 0.9rem;
  color: var(--primary-text-color);
  font-weight: 500;
}
</style>