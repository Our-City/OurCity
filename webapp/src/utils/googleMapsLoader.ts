/**
 * Google Maps API loader utility
 * Ensures the Google Maps script is loaded only once and shared across components
 */

let loadPromise: Promise<void> | null = null;
let isLoaded = false;

export function loadGoogleMaps(): Promise<void> {
  // If already loaded and Places library is ready, return immediately
  if (isLoaded && typeof google !== 'undefined' && google.maps && google.maps.places) {
    return Promise.resolve();
  }

  // If currently loading, return the existing promise
  if (loadPromise) {
    return loadPromise;
  }

  // Start loading
  loadPromise = new Promise((resolve, reject) => {
    // Check if already loaded (in case another component loaded it)
    if (typeof google !== 'undefined' && google.maps && google.maps.places) {
      isLoaded = true;
      resolve();
      return;
    }

    // Create and append script
    const script = document.createElement('script');
    script.src = `https://maps.googleapis.com/maps/api/js?key=${import.meta.env.VITE_GOOGLE_MAPS_API_KEY}&libraries=places,geocoding,marker&v=weekly&loading=async`;
    script.async = true;
    script.defer = true;

    script.onload = () => {
      // Poll until Places library is fully available
      const checkPlacesLibrary = () => {
        if (typeof google !== 'undefined' && 
            google.maps && 
            google.maps.places && 
            google.maps.places.Autocomplete) {
          isLoaded = true;
          console.log('Google Maps and Places API fully loaded');
          resolve();
        } else {
          // Check again after a short delay
          setTimeout(checkPlacesLibrary, 50);
        }
      };

      checkPlacesLibrary();

      // Timeout after 10 seconds
      setTimeout(() => {
        if (!isLoaded) {
          loadPromise = null;
          reject(new Error('Timeout waiting for Google Maps Places library'));
        }
      }, 10000);
    };

    script.onerror = () => {
      loadPromise = null; // Reset so it can be retried
      reject(new Error('Failed to load Google Maps script'));
    };

    document.head.appendChild(script);
  });

  return loadPromise;
}

export function isGoogleMapsLoaded(): boolean {
  return isLoaded && 
         typeof google !== 'undefined' && 
         typeof google.maps !== 'undefined' &&
         typeof google.maps.places !== 'undefined';
}