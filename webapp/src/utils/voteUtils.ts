/*
  /webapp/src/utils/voteUtils.ts

  Utility function for handling vote types. If the vote type is represented as a string or number,
  this function converts it to the VoteType enum.
*/
import { VoteType } from "@/types/enums";

export function parseVoteType(value: string | number | null | undefined): VoteType {
  switch (value) {
    case "Upvote":
    case 1:
      return VoteType.UPVOTE;
    case "Downvote":
    case -1:
      return VoteType.DOWNVOTE;
    case "NoVote":
    case 0:
    default:
      return VoteType.NOVOTE;
  }
}
