/*
Used ChatGPT to help with creation of this file (general structure + how to track request timing)
 */

import http from "k6/http";
import { Trend } from "k6/metrics";

export const options = {
  vus: 200,
  duration: "2m",
};

const getPostsTrend = new Trend("duration_get_posts");

export default function () {
  getPosts();
}

function getPosts() {
  const res = http.get("http://host.docker.internal:8000/apis/v1/posts");

  getPostsTrend.add(res.timings.duration);
}
