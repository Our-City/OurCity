/**
 * Generative AI was used to assist in the creation of this file
 * Location validation utilities for Winnipeg city boundaries
 */

// Winnipeg city center coordinates
const WINNIPEG_CENTER = {
  latitude: 49.8951,
  longitude: -97.1384,
};

// Radius in kilometers (covers Winnipeg and suburbs)
const WINNIPEG_RADIUS_KM = 25;

/**
 * Calculates distance between two coordinates using Haversine formula
 * Returns distance in kilometers
 */
export function calculateDistance(lat1: number, lon1: number, lat2: number, lon2: number): number {
  // Validate inputs
  if (!isValidCoordinate(lat1, lon1) || !isValidCoordinate(lat2, lon2)) {
    return Infinity;
  }

  const R = 6371; // Earth's radius in kilometers
  const dLat = toRadians(lat2 - lat1);
  const dLon = toRadians(lon2 - lon1);

  const a =
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(toRadians(lat1)) * Math.cos(toRadians(lat2)) * Math.sin(dLon / 2) * Math.sin(dLon / 2);

  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  const distance = R * c;

  return distance;
}

function toRadians(degrees: number): number {
  return degrees * (Math.PI / 180);
}

/**
 * Validate if coordinates are valid numbers
 */
function isValidCoordinate(lat: number, lng: number): boolean {
  return (
    typeof lat === "number" &&
    typeof lng === "number" &&
    !isNaN(lat) &&
    !isNaN(lng) &&
    isFinite(lat) &&
    isFinite(lng)
  );
}

/**
 * Check if coordinates are within Winnipeg city boundaries
 * @param latitude - Latitude coordinate
 * @param longitude - Longitude coordinate
 * @returns true if location is within Winnipeg, false otherwise
 */
export function isWithinWinnipeg(latitude: number, longitude: number): boolean {
  // Validate inputs
  if (!isValidCoordinate(latitude, longitude)) {
    return false;
  }

  const distance = calculateDistance(
    latitude,
    longitude,
    WINNIPEG_CENTER.latitude,
    WINNIPEG_CENTER.longitude,
  );

  return distance <= WINNIPEG_RADIUS_KM;
}

/**
 * Get distance from Winnipeg city center in kilometers
 */
export function getDistanceFromWinnipeg(latitude: number, longitude: number): number {
  // Validate inputs
  if (!isValidCoordinate(latitude, longitude)) {
    return Infinity;
  }

  return calculateDistance(
    latitude,
    longitude,
    WINNIPEG_CENTER.latitude,
    WINNIPEG_CENTER.longitude,
  );
}

/**
 * Validate location and return error message if invalid
 */
export function validateWinnipegLocation(latitude?: number, longitude?: number): string | null {
  if (!latitude || !longitude) {
    return null;
  }

  if (!isWithinWinnipeg(latitude, longitude)) {
    const distance = getDistanceFromWinnipeg(latitude, longitude);
    return `Selected location is ${distance.toFixed(1)} km from Winnipeg. Please select a location within Winnipeg city limits.`;
  }

  return null;
}
