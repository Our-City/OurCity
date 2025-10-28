import type { UserResponseDto } from "@/types/users";
import type { PostResponseDto } from "@/types/posts";
import type { CommentResponseDto } from "@/types/comments";

export const mockUsers: UserResponseDto[] = [
  {   
    id: 1, 
    username: "a_real_prof", 
    displayName: "a real prof",
    postIds: [ 1 ],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
  { 
    id: 2, 
    username: "PoStoreGangEyeSayShun", 
    displayName: "",
    postIds: [ 2 ],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
  {
    id: 3,
    username: "ViewCompOwnItLieBrrrAiry",
    displayName: "",
    postIds: [],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
  {
    id: 4,
    username: "RealMichaelJordanProbably",
    displayName: "",
    postIds: [],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
  {
    id: 5,
    username: "UnemployedComputerScientist",
    displayName: "",
    postIds: [],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
  {
    id: 6,
    username: "tacobell_nachofries_supreme",
    displayName: "",
    postIds: [],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
  {
    id: 7,
    username: "Shopoholic",
    displayName: "",
    postIds: [ 7 ],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
  {
    id: 8,
    username: "kingoftouchinggrass",
    displayName: "GrassKing",
    postIds: [],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: true,
  },
  {
    id: 9,
    username: "kraftdinnermaster",
    displayName: "Kraft Dinner Master",
    postIds: [],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: true,
  },
  {
    id: 10,
    username: "YoungSheldonBazinga",
    displayName: "",
    postIds: [],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
  {
    id: 11,
    username: "ILoveGambling123",
    displayName: "",
    postIds: [],
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
    isDeleted: false,
  },
];

export const mockPosts: PostResponseDto[] = [
  {
    id: 1,
    authorId: 1,
    title: "Street Construction around the University",
    description: "What is up with all of this street construction around the University? It takes me like double the time it usually does to go home everyday now. Is there actually like a legitimate reason why there is so much road construction in general in Winnipeg? It's always like this too in the summer and it feels like they redo the same road every year.",
    location: "University of Manitoba",
    images: [
      {
        url: "https://images.unsplash.com/photo-1581094271901-8022df4466f9?w=800&auto=format&fit=crop"
      }
    ],
    commentIds: [1, 2, 3],
    votes: 721,
  },
  {
    id: 2,
    authorId: 2,
    title: "Exploring the City",
    description: "I had so much fun last weekend around Downtown! I don't really understand why people are always saying that there's nothing to do in Winnipeg. Especially in the Exchange district, I feel like I could literally just spend hours walking around and trying new things.",
    location: "Downtown",
    images: [
      {
        url: "https://images.unsplash.com/photo-1477959858617-67f85cf4f1df?w=800&auto=format&fit=crop"
      }
    ],
    commentIds: [4, 5, 6, 7],
    votes: 426,
  },
  {
    id: 3,
    authorId: 3,
    title: "Shopping Malls",
    description: "I'm very excited for the Portal Place redevelopments! It feels like that place has been dead for a while now...",
    location: "Portage Place",
    images: [
      {
        url: "https://images.unsplash.com/photo-1555529902-5261145633bf?w=800&auto=format&fit=crop"
      }
    ],
    commentIds: [],
    votes: 135,
  },
];

export const mockComments: CommentResponseDto[] = [
  {
    id: 1,
    authorId: 3,
    postId: 1,
    content: "Apparently (don't quote me on this) I was told that it's because construction companies bid for road contracts and tend to bid on more contracts than they can handle, so they end up with a lot of projects going on at once.",
    votes: 32,
    isDeleted: false,
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
  },
  {
    id: 2,
    authorId: 4,
    postId: 1,
    content: "the bus is even worse :(",
    votes: 65,
    isDeleted: false,
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
  },
  {
    id: 3,
    authorId: 5,
    postId: 2,
    content: "Have you tried Jenna Rae Cakes at the Forks? I want to try it, but I haven't gotten around to it yet",
    votes: 3,
    isDeleted: false,
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
  },
  {
    id: 4,
    authorId: 6,
    postId: 2,
    content: "I wish they would open more Taco Bells around there.",
    votes: -4,
    isDeleted: false,
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
  },
  {
    id: 5,
    authorId: 10,
    postId: 2,
    content: "I prefer to just watch young sheldon at home.",
    votes: -10,
    isDeleted: false,
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
  },
  {
    id: 6,
    authorId: 11,
    postId: 2,
    content: "Have you been to the casino?",
    votes: 111,
    isDeleted: false,
    createdAt: "2025-10-01T12:00:00Z",
    updatedAt: "2025-10-01T12:00:00Z",
  }
];