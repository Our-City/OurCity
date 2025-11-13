<!-- Google Maps component for selecting a location by clicking on the map
  Allows users to place a marker and get location details 
  Generative AI - CoPilot was used to assist in the creation of this file.
    CoPilot was asked to provide help with CSS styling and for help with syntax.
    It also assisted with error handling-->
<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { loadGoogleMaps } from "@/utils/googleMapsLoader";

interface Props {
  modelValue?: {
    latitude: number;
    longitude: number;
    name?: string;
  } | null;
  height?: string;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: null,
  height: "400px",
});

const emit = defineEmits<{
  "update:modelValue": [
    value: {
      latitude: number;
      longitude: number;
      name: string;
    } | null,
  ];
}>();

// Refs for map and marker
const mapContainer = ref<HTMLDivElement | null>(null);
let map: google.maps.Map | null = null;
let marker: google.maps.marker.AdvancedMarkerElement | null = null;
let geocoder: google.maps.Geocoder | null = null;

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

    // Load Google Maps using shared utility
    await loadGoogleMaps();

    // Small delay to ensure all libraries are ready
    await new Promise(resolve => setTimeout(resolve, 100));

    // Initialize geocoder
    geocoder = new google.maps.Geocoder();

    // Determine initial center
    const initialCenter =
      props.modelValue?.latitude && props.modelValue?.longitude
        ? { lat: props.modelValue.latitude, lng: props.modelValue.longitude }
        : DEFAULT_CENTER;

    // Create map instance with Map ID (required for Advanced Markers)
    map = new google.maps.Map(mapContainer.value, {
      center: initialCenter,
      zoom: 12,
      mapTypeControl: true,
      streetViewControl: false,
      fullscreenControl: true,
      mapId: "OURCITY_MAP", // Required for Advanced Markers
    });

    // Add click listener to map
    map.addListener("click", (event: google.maps.MapMouseEvent) => {
      if (event.latLng) {
        handleMapClick(event.latLng);
      }
    });

    // If there's an initial value, place a marker
    if (props.modelValue?.latitude && props.modelValue?.longitude) {
      placeMarker(initialCenter);
    }

    isLoading.value = false;
  } catch (err) {
    console.error("Error loading Google Maps:", err);
    error.value = "Failed to load map. Please check your API key and internet connection.";
    isLoading.value = false;
  }
}
// Handle map click - place marker and get location details
async function handleMapClick(latLng: google.maps.LatLng) {
  const lat = latLng.lat();
  const lng = latLng.lng();

  // Place or move marker
  placeMarker({ lat, lng });

  // Reverse geocode to get location name
  const locationName = await reverseGeocode(lat, lng);

  // Emit the selected location
  emit("update:modelValue", {
    latitude: lat,
    longitude: lng,
    name: locationName,
  });
}

// Place or update marker on map using AdvancedMarkerElement
function placeMarker(position: { lat: number; lng: number }) {
  if (!map) return;

  if (marker) {
    // Update existing marker position
    marker.position = position;
  } else {
    // Create new Advanced Marker
    marker = new google.maps.marker.AdvancedMarkerElement({
      map,
      position,
      gmpDraggable: true,
      title: "Selected Location",
    });

    // Add drag listener to marker
    marker.addListener("dragend", () => {
      const newPosition = marker?.position;
      if (newPosition && typeof newPosition === 'object' && 'lat' in newPosition && 'lng' in newPosition) {
        const lat = typeof newPosition.lat === 'function' ? newPosition.lat() : newPosition.lat;
        const lng = typeof newPosition.lng === 'function' ? newPosition.lng() : newPosition.lng;
        
        // Create a proper LatLng object
        const latLng = new google.maps.LatLng(lat as number, lng as number);
        handleMapClick(latLng);
      }
    });
  }

  // Center map on marker
  map.panTo(position);
}

// Reverse geocode coordinates to get location name
async function reverseGeocode(lat: number, lng: number): Promise<string> {
  if (!geocoder) {
    return `${lat.toFixed(6)}, ${lng.toFixed(6)}`;
  }

  try {
    const response = await geocoder.geocode({
      location: { lat, lng },
    });

    if (response.results && response.results.length > 0) {
      // Get the most specific address available
      const result = response.results[0];
      return result.formatted_address;
    }

    return `${lat.toFixed(6)}, ${lng.toFixed(6)}`;
  } catch (err) {
    console.error("Geocoding error:", err);
    return `${lat.toFixed(6)}, ${lng.toFixed(6)}`;
  }
}

// Clear the selected location
function clearLocation() {
  if (marker) {
    marker.map = null; // Remove marker from map
    marker = null;
  }
  emit("update:modelValue", null);
}

// Watch for external changes to modelValue
watch(
  () => props.modelValue,
  (newValue, oldValue) => {
    // Only update if the value actually changed
    if (
      newValue?.latitude !== oldValue?.latitude ||
      newValue?.longitude !== oldValue?.longitude
    ) {
      if (newValue?.latitude && newValue?.longitude && map) {
        placeMarker({ lat: newValue.latitude, lng: newValue.longitude });
        
        // Optionally, re-center the map on the new location
        map.setCenter({ lat: newValue.latitude, lng: newValue.longitude });
        map.setZoom(15); // Zoom in to show the location better
      } else if (!newValue && marker) {
        clearLocation();
      }
    }
  },
  { deep: true } // Watch nested properties
);

// Initialize map on mount
onMounted(() => {
  initMap();
});

// Expose clear method to parent
defineExpose({
  clearLocation,
});
</script>

<template>
  <div class="map-picker">
    <div v-if="isLoading" class="map-loading">
      <i class="pi pi-spin pi-spinner"></i>
      <p>Loading map...</p>
    </div>

    <div v-else-if="error" class="map-error">
      <i class="pi pi-exclamation-triangle"></i>
      <p>{{ error }}</p>
    </div>

    <div
      v-show="!isLoading && !error"
      ref="mapContainer"
      class="map-container"
      :style="{ height: props.height }"
    ></div>

    <div v-if="modelValue && !isLoading" class="map-info">
      <div class="location-details">
        <i class="pi pi-map-marker"></i>
        <span class="location-name">{{ modelValue.name }}</span>
      </div>
      <button type="button" class="clear-button" @click="clearLocation">
        <i class="pi pi-times"></i>
        Clear Location
      </button>
    </div>
  </div>
</template>

<style scoped>
.map-picker {
  width: 100%;
  border: 1px solid var(--border-color);
  border-radius: 0.5rem;
  overflow: hidden;
}

.map-container {
  width: 100%;
  min-height: 300px;
}

.map-loading,
.map-error {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 400px;
  gap: 1rem;
  color: var(--secondary-text-color);
}

.map-loading i,
.map-error i {
  font-size: 3rem;
}

.map-error {
  color: var(--error-color, #dc3545);
}

.map-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: var(--neutral-color);
  border-top: 1px solid var(--border-color);
}

.location-details {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
}

.location-details i {
  color: var(--primary-color, #3b82f6);
}

.location-name {
  font-size: 0.9rem;
  color: var(--primary-text-color);
  font-weight: 500;
}

.clear-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background: transparent;
  border: 1px solid var(--border-color);
  border-radius: 0.25rem;
  color: var(--secondary-text-color);
  cursor: pointer;
  transition: all 0.2s;
}

.clear-button:hover {
  background: var(--error-color, #dc3545);
  color: white;
  border-color: var(--error-color, #dc3545);
}
</style>