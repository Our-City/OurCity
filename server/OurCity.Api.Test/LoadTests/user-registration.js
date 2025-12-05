/*
Used ChatGPT to help with creation of this file (general structure + how to track request timing)
 */

import http from "k6/http";
import { Trend } from "k6/metrics";

export const options = {
  stages: [
    { duration: "1m", target: 200 },
    { duration: "1m", target: 100 },
    { duration: "1m", target: 50 },
  ],
};

const createUserTrend = new Trend("duration_create_user");

export default function () {
  const username = crypto.randomUUID().toString();
  const password = "TestPassword1!";

  createUser(username, password);
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
}
