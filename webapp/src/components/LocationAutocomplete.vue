<script setup lang="ts">
import { ref, onMounted, watch, nextTick } from "vue";
import { loadGoogleMaps } from "@/utils/googleMapsLoader";

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
}>();

// Use a plain HTML input ref instead of PrimeVue component
const inputRef = ref<HTMLInputElement | null>(null);
const searchValue = ref(props.modelValue);
let autocomplete: google.maps.places.Autocomplete | null = null;

// Initialize Google Places Autocomplete
async function initAutocomplete() {
  try {
    console.log("LocationAutocomplete: Starting initialization...");
    
    // Load Google Maps using shared utility (will wait for Places library)
    await loadGoogleMaps();
    
    console.log("LocationAutocomplete: Google Maps loaded");

    // Wait for next tick to ensure DOM is ready
    await nextTick();

    if (!inputRef.value) {
      console.error("LocationAutocomplete: Input element not found");
      return;
    }

    console.log("LocationAutocomplete: Input element found:", inputRef.value);

    // Double-check that Places library is available
    if (!google.maps.places || !google.maps.places.Autocomplete) {
      throw new Error("Google Maps Places library not loaded");
    }

    console.log("LocationAutocomplete: Creating Autocomplete instance...");

    // Create autocomplete instance
    autocomplete = new google.maps.places.Autocomplete(inputRef.value, {
      types: ["geocode", "establishment"],
      componentRestrictions: { country: "ca" },
      fields: ["formatted_address", "geometry", "name"],
    });

    console.log("LocationAutocomplete: Autocomplete created successfully:", autocomplete);

    // Listen for place selection
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

  console.log("LocationAutocomplete: Emitting location:", { locationName, lat, lng });

  // Update the search value
  searchValue.value = locationName;

  // Emit the location name to parent
  emit("update:modelValue", locationName);

  // Emit the full location details (name, lat, lng)
  emit("location-selected", {
    name: locationName,
    latitude: lat,
    longitude: lng,
  });
}

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
  // Wait a bit to ensure MapPicker has started loading Google Maps
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

/* Override Google's autocomplete dropdown styles to match your theme */
:global(.pac-container) {
  border-radius: 0.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  margin-top: 0.25rem;
  font-family: inherit;
  z-index: 9999; /* Ensure it appears above other elements */
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