/**
 * Removes Canadian postal codes from location strings
 * Canadian postal code format: A1A 1A1 (letter-digit-letter space digit-letter-digit)
 */
export function removePostalCode(location?: string): string {
  if (!location) return '';
  
  // Canadian postal code regex pattern
  // Matches: R3H 0S2, R3T, etc.
  const postalCodePattern = /[A-Z]\d[A-Z]\s?\d[A-Z]\d/gi;
  
  // Remove postal code and clean up extra spaces/commas
  return location
    .replace(postalCodePattern, '')
    .replace(/\s+,/g, ',') // Remove spaces before commas
    .replace(/,\s+,/g, ',') // Remove double commas with spaces
    .replace(/,\s*,/g, ',') // Remove double commas
    .replace(/,\s*$/, '') // Remove trailing comma
    .replace(/\s+/g, ' ') // Normalize multiple spaces to single space
    .trim();
}