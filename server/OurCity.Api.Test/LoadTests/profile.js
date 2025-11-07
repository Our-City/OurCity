/*
Used ChatGPT to help with creation of this file (particularly, how to track request timing)
 */

import http from 'k6/http';
import { Trend } from 'k6/metrics';

export const options = {
    vus: 10,
    duration: "1s"
};

const createUserTrend = new Trend('duration_create_user');
const updateUserTrend = new Trend('duration_update_user');
const loginTrend = new Trend('duration_login');
const getPostsTrend = new Trend('duration_posts');
const getMeTrend = new Trend('duration_me');
//create post
//get post by id
//update post
//delete post
//vote on post
//get tags
//delete user
//update me
//delete me
//create comment
//get comments
//update comment
//delete comment
//vote on comment
//can create posts
//can update posts
//logout
//auth/me
//ALL MEDIA ENDPOINTS
//all the endpoints should be there

export default function () {
    const username = crypto.randomUUID().toString();
    const password = "TestPassword1!";
    const newUsername = crypto.randomUUID().toString();
    
    const createdUserId = createUser(username, password);
    updateUser(createdUserId, newUsername);
    login(newUsername, password);
    getPosts();
    getMe();
}

function createUser(username, password) {
    const payload = JSON.stringify({ username: username, password: password });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post('http://host.docker.internal:8000/apis/v1/users', payload, params);

    createUserTrend.add(res.timings.duration);
    
    return res.json().id;
}

function updateUser(userId, newUsername) {
    const payload = JSON.stringify({ username: newUsername });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.put(`http://host.docker.internal:8000/apis/v1/users/${userId}`, payload, params);

    updateUserTrend.add(res.timings.duration);
}

function login(username, password) {
    const payload = JSON.stringify({ username: username, password: password });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        }
    };

    const res = http.post('http://host.docker.internal:8000/apis/v1/authentication/login', payload, params);

    loginTrend.add(res.timings.duration);
}

function getPosts() {
    const res = http.get('http://host.docker.internal:8000/apis/v1/posts', { tags: { endpoint: 'posts' }});

    getPostsTrend.add(res.timings.duration);
}

function getMe() {
    const res = http.get('http://host.docker.internal:8000/apis/v1/me', { tags: { endpoint: 'me' }});

    getMeTrend.add(res.timings.duration);
}