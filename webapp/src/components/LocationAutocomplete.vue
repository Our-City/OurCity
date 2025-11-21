<!-- Generative AI - CoPilot was used to assist in the creation of this file.
  CoPilot was asked to provide help with CSS styling and for help with syntax.
  It also assisted with error handling.-->

<script setup lang="ts">
import { ref, onMounted, watch, nextTick } from "vue";
import { loadGoogleMaps } from "@/utils/googleMapsLoader";
import { isWithinWinnipeg, getDistanceFromWinnipeg } from "@/utils/locationValidator";

interface Props {
  modelValue?: string;
  placeholder?: string;
  disabled?: boolean;
}

interface LocationResult {
  name: string;
  latitude: number;
  longitude: number;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: "",
  placeholder: "Search for a location...",
  disabled: false,
});

const emit = defineEmits<{
  "update:modelValue": [value: string];
  "location-selected": [location: LocationResult];
  "location-error": [error: string | null];
}>();

const inputRef = ref<HTMLInputElement | null>(null);
const searchValue = ref(props.modelValue);
let autocomplete: google.maps.places.Autocomplete | null = null;

// Initialize Google Places Autocomplete
async function initAutocomplete() {
  try {
    console.log("LocationAutocomplete: Starting initialization...");
    
    await loadGoogleMaps();
    
    console.log("LocationAutocomplete: Google Maps loaded");

    await nextTick();

    if (!inputRef.value) {
      console.error("LocationAutocomplete: Input element not found");
      return;
    }

    console.log("LocationAutocomplete: Input element found:", inputRef.value);

    if (!google.maps.places || !google.maps.places.Autocomplete) {
      throw new Error("Google Maps Places library not loaded");
    }

    console.log("LocationAutocomplete: Creating Autocomplete instance...");

    autocomplete = new google.maps.places.Autocomplete(inputRef.value, {
      types: ["geocode", "establishment"],
      componentRestrictions: { country: "ca" },
      fields: ["formatted_address", "geometry", "name"],
    });

    console.log("LocationAutocomplete: Autocomplete created successfully:", autocomplete);

    autocomplete.addListener("place_changed", handlePlaceSelect);
    
    console.log("LocationAutocomplete: Place change listener added");
  } catch (error) {
    console.error("LocationAutocomplete: Error initializing autocomplete:", error);
  }
}

// Handle place selection from autocomplete
function handlePlaceSelect() {
  console.log("LocationAutocomplete: Place changed event triggered");
  
  if (!autocomplete) {
    console.error("LocationAutocomplete: Autocomplete not initialized");
    return;
  }

  const place = autocomplete.getPlace();
  console.log("LocationAutocomplete: Selected place:", place);

  if (!place.geometry || !place.geometry.location) {
    console.error("LocationAutocomplete: No geometry found for selected place");
    return;
  }

  const locationName = place.formatted_address || place.name || "";
  const lat = place.geometry.location.lat();
  const lng = place.geometry.location.lng();

  // Validate location is within Winnipeg
  if (!isWithinWinnipeg(lat, lng)) {
    const distance = getDistanceFromWinnipeg(lat, lng);
    const errorMsg = `"${locationName}" is ${distance.toFixed(1)} km from Winnipeg. Please select a location within Winnipeg.`;
    emit("location-error", errorMsg);
    
    // Clear the input
    searchValue.value = "";
    return;
  }

  emit("location-error", null);

  console.log("LocationAutocomplete: Emitting location:", { locationName, lat, lng });

  searchValue.value = locationName;

  emit("update:modelValue", locationName);

  emit("location-selected", {
    name: locationName,
    latitude: lat,
    longitude: lng,
  });
}

watch(
  () => props.modelValue,
  (newValue) => {
    if (newValue !== searchValue.value) {
      searchValue.value = newValue;
      
      console.log("LocationAutocomplete: External value changed to:", newValue);
    }
  },
);

// Handle manual input changes
function handleInput(event: Event) {
  const target = event.target as HTMLInputElement;
  searchValue.value = target.value;
  emit("update:modelValue", target.value);
  console.log("LocationAutocomplete: Input changed to:", target.value);
}

// Watch for external changes to modelValue
watch(
  () => props.modelValue,
  (newValue) => {
    if (newValue !== searchValue.value) {
      searchValue.value = newValue;
      console.log("LocationAutocomplete: External value changed to:", newValue);
    }
  },
);

onMounted(async () => {
  console.log("LocationAutocomplete: Component mounted");
  await new Promise(resolve => setTimeout(resolve, 500));
  await initAutocomplete();
});
</script>

<template>
  <div class="location-autocomplete">
    <input
      ref="inputRef"
      v-model="searchValue"
      type="text"
      :placeholder="props.placeholder"
      :disabled="props.disabled"
      class="location-input p-inputtext"
      :class="{ 'p-invalid': locationError }" 
      @input="handleInput"
    />
  </div>
</template>

<style scoped>
.location-autocomplete {
  width: 100%;
}

.location-input {
  width: 100%;
  padding: 0.75rem;
  font-size: 1rem;
  border: 1px solid var(--border-color);
  border-radius: 0.5rem;
  transition: border-color 0.2s;
  font-family: inherit;
}

.location-input:focus {
  outline: none;
  border-color: var(--primary-color, #3b82f6);
  box-shadow: 0 0 0 0.2rem rgba(59, 130, 246, 0.25);
}

.location-input:disabled {
  background-color: var(--disabled-bg-color, #e9ecef);
  cursor: not-allowed;
}

:global(.pac-container) {
  border-radius: 0.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  margin-top: 0.25rem;
  font-family: inherit;
  z-index: 9999; 
}

:global(.pac-item) {
  padding: 0.75rem 1rem;
  cursor: pointer;
  border-top: 1px solid var(--border-color);
}

:global(.pac-item:first-child) {
  border-top: none;
}

:global(.pac-item:hover) {
  background-color: var(--neutral-color);
}

:global(.pac-icon) {
  margin-right: 0.5rem;
}

:global(.pac-item-query) {
  font-weight: 600;
  color: var(--primary-text-color);
}

:global(.pac-matched) {
  font-weight: 700;
}
</style>