/*
Used ChatGPT to help with creation of this file (general structure + how to track request timing)
 */

import http from "k6/http";
import { Trend } from "k6/metrics";

export const options = {
  vus: 125,
  duration: "2m",
};

const createUserTrend = new Trend("duration_create_user");
const loginTrend = new Trend("duration_login");
const createPostTrend = new Trend("duration_create_post");
const votePostTrend = new Trend("duration_vote_post");
const createCommentTrend = new Trend("duration_create_comment");
const voteCommentTrend = new Trend("duration_vote_comment");
const logoutTrend = new Trend("duration_logout");

export default function () {
  const username = crypto.randomUUID().toString();
  const password = "TestPassword1!";

  const createdUserId = createUser(username, password);
  login(username, password);
  const createdPostId = createPost();
  votePost(createdPostId);
  const createdCommentId = createComment(createdPostId);
  voteComment(createdCommentId);
  logout();
}

function createUser(username, password) {
  const payload = JSON.stringify({ username: username, password: password });
  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  const res = http.post(
    "http://host.docker.internal:8000/apis/v1/users",
    payload,
    params
  );

  createUserTrend.add(res.timings.duration);

  return res.json().id;
}

function login(username, password) {
  const payload = JSON.stringify({ username: username, password: password });
  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  const res = http.post(
    "http://host.docker.internal:8000/apis/v1/authentication/login",
    payload,
    params
  );

  loginTrend.add(res.timings.duration);
}

function createPost() {
  const payload = JSON.stringify({
    title: "Test Title",
    description: "Test Description",
    tags: [],
  });
  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  const res = http.post(
    "http://host.docker.internal:8000/apis/v1/posts",
    payload,
    params
  );

  createPostTrend.add(res.timings.duration);

  return res.json().id;
}

function votePost(postId) {
  const payload = JSON.stringify({ voteType: "Upvote" });
  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  const res = http.put(
    `http://host.docker.internal:8000/apis/v1/posts/${postId}/votes`,
    payload,
    params
  );

  votePostTrend.add(res.timings.duration);
}

function createComment(postId) {
  const payload = JSON.stringify({ content: "Test Content" });
  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  const res = http.post(
    `http://host.docker.internal:8000/apis/v1/posts/${postId}/comments`,
    payload,
    params
  );

  createCommentTrend.add(res.timings.duration);

  return res.json().id;
}

function voteComment(commentId) {
  const payload = JSON.stringify({ voteType: "Upvote" });
  const params = {
    headers: {
      "Content-Type": "application/json",
    },
  };

  const res = http.put(
    `http://host.docker.internal:8000/apis/v1/comments/${commentId}/votes`,
    payload,
    params
  );

  voteCommentTrend.add(res.timings.duration);
}

function logout() {
  const res = http.post(
    "http://host.docker.internal:8000/apis/v1/authentication/logout"
  );

  logoutTrend.add(res.timings.duration);
}
