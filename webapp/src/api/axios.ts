/*
  /webapp/src/api/axios.ts
  
  This file sets up a preconfigured axios instance for making HTTP requests to the backend API.
  It includes a response interceptor to automatically unwrap standard API responses.

  AI assisted in the creation of this file.
  Specifically, ChatGPT helped to write the response interceptor logic as well as error handling.
*/
import axios from "axios";


// create preconfigured axios instance
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL, // should be localhost:8000/apis/v1
  withCredentials: true, // backend uses cookies for auth
});

// response interceptor to unwrap backend API responses automatically.
api.interceptors.response.use(
  (response) => {
    const data = response.data; 

    // check if backend returned the Result response wrapper
    if (
      data &&
      typeof data === "object" &&
      ("isSuccess" in data || "IsSuccess" in data)
    ) {
      const isSuccess = data.isSuccess ?? data.IsSuccess;
      const payload = data.data ?? data.Data;
      const error = data.error ?? data.Error;

      if (!isSuccess) {
        // convert backend error to js error
        return Promise.reject(new Error(error || "Request failed."));
      }

      // unwrap the payload so API calls get it directly
      response.data = payload;
    }

    return response;
  },
  (error) => {
    let message = "An unexpected error occurred.";

    if (error.response) {
      const status = error.response.status;
      const data = error.response.data;

      // try to extract meaningful message from backend response
      if (typeof data === 'string') {
        message = data;
      } else if (data?.detail) {
        message = data.detail;
      } else if (data?.error) {
        message = data.error;
      } else if (data?.title) {
        message = data.title;
      }

      // handle specific HTTP status code feedback
      if (status === 401) {
        message = "Unauthorized. Please log in.";
      } else if (status === 403) {
        message = "Forbidden. You don't have permission to access this resource.";
      } else if (status === 404) {
        message = "Resource not found.";
      } else if (status >= 500) {
        message = "Server error. Please try again later.";
      }
    }

    // maybe add a toast notification here too
    return Promise.reject(new Error(message));
  }
);

export default api;
