<!-- Google Maps component for selecting a location by clicking on the map
  Allows users to place a marker and get location details -->
<script setup lang="ts">
import { ref, onMounted, watch } from "vue";

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

// Check if Google Maps script is already loaded
function isGoogleMapsLoaded(): boolean {
  return typeof google !== 'undefined' && typeof google.maps !== 'undefined';
}

// Wait for Google Maps to be fully loaded
function waitForGoogleMaps(): Promise<void> {
  return new Promise((resolve) => {
    if (isGoogleMapsLoaded()) {
      resolve();
      return;
    }
    
    const checkInterval = setInterval(() => {
      if (isGoogleMapsLoaded()) {
        clearInterval(checkInterval);
        resolve();
      }
    }, 100);
  });
}

// Initialize Google Maps using the new functional API
async function initMap() {
  try {
    isLoading.value = true;
    error.value = null;

    if (!mapContainer.value) {
      throw new Error("Map container not found");
    }

    // Only load the script if it hasn't been loaded yet
    if (!isGoogleMapsLoaded()) {
      const script = document.createElement("script");
      script.src = `https://maps.googleapis.com/maps/api/js?key=${import.meta.env.VITE_GOOGLE_MAPS_API_KEY}&libraries=places,geocoding,marker&v=weekly&loading=async`;
      script.async = true;
      script.defer = true;

      document.head.appendChild(script);
      
      // Wait for Google Maps to be fully loaded
      await waitForGoogleMaps();
    }

    // Additional wait to ensure all libraries are loaded
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
  (newValue) => {
    if (newValue?.latitude && newValue?.longitude && map) {
      placeMarker({ lat: newValue.latitude, lng: newValue.longitude });
    } else if (!newValue && marker) {
      clearLocation();
    }
  },
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