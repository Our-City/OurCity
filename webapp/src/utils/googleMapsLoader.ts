/**
 * Generative AI was used to assist in the creation of this file
 * Google Maps API loader utility
 * Ensures the Google Maps script is loaded only once and shared across components
 */

let loadPromise: Promise<void> | null = null;
let isLoaded = false;

export function loadGoogleMaps(): Promise<void> {
  if (isLoaded && typeof google !== "undefined" && google.maps && google.maps.places) {
    return Promise.resolve();
  }

  if (loadPromise) {
    return loadPromise;
  }

  // Start loading
  loadPromise = new Promise((resolve, reject) => {
    if (typeof google !== "undefined" && google.maps && google.maps.places) {
      isLoaded = true;
      resolve();
      return;
    }

    const script = document.createElement("script");
    // Add 'visualization' to the libraries parameter
    script.src = `https://maps.googleapis.com/maps/api/js?key=${import.meta.env.VITE_GOOGLE_MAPS_API_KEY}&libraries=places,geocoding,marker,visualization&v=weekly&loading=async`;
    script.async = true;
    script.defer = true;

    script.onload = () => {
      const checkPlacesLibrary = () => {
        if (
          typeof google !== "undefined" &&
          google.maps &&
          google.maps.places &&
          google.maps.places.Autocomplete &&
          google.maps.visualization && // Check visualization library is loaded
          google.maps.visualization.HeatmapLayer
        ) {
          isLoaded = true;
          console.log("Google Maps, Places API, and Visualization library fully loaded");
          resolve();
        } else {
          setTimeout(checkPlacesLibrary, 50);
        }
      };

      checkPlacesLibrary();

      // Timeout after 10 seconds
      setTimeout(() => {
        if (!isLoaded) {
          loadPromise = null;
          reject(new Error("Timeout waiting for Google Maps libraries"));
        }
      }, 10000);
    };

    script.onerror = () => {
      loadPromise = null;
      reject(new Error("Failed to load Google Maps script"));
    };

    document.head.appendChild(script);
  });

  return loadPromise;
}

export function isGoogleMapsLoaded(): boolean {
  return (
    isLoaded &&
    typeof google !== "undefined" &&
    typeof google.maps !== "undefined" &&
    typeof google.maps.places !== "undefined" &&
    typeof google.maps.visualization !== "undefined"
  );
}
