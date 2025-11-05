/*
  /webapp/src/models/dtos/tag.ts

  This file contains the Data Transfer Objects (DTOs) related to the Tag entity.
  The types corresponding to each DTO are defined as per the request/response definitions in the API contract.
*/

/*
  why interface instead of type? interfaces will be extensible for future changes, types won't.
  if we end up moving to classes later because we end up needing object behaviour, interfaces will make that easier.
*/
export interface TagDto {
  id: string;
  name: string;
}
