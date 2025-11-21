/*
Used ChatGPT to help with creation of this file (particularly, how to track request timing, uploading files, etc)
 */

import http from 'k6/http';
import { Trend } from 'k6/metrics';

export const options = {
    vus: 10,
    duration: "1s"
};

const createUserTrend = new Trend('duration_create_user');
const updateUserTrend = new Trend('duration_update_user');
const updateMeTrend = new Trend('duration_update_me');
const loginTrend = new Trend('duration_login');
const getMeAuthTrend = new Trend('duration_get_me_auth');
const createPostTrend = new Trend('duration_create_post');
const getPostByIdTrend = new Trend('duration_get_post_by_id');
const updatePostTrend = new Trend('duration_update_post');
const votePostTrend = new Trend('duration_vote_post');
const createCommentTrend = new Trend('duration_create_comment');
const updateCommentTrend = new Trend('duration_update_comment');
const voteCommentTrend = new Trend('duration_vote_comment');
const getPostsTrend = new Trend('duration_get_posts');
const getCommentsTrend = new Trend('duration_get_comments');
const getMeUserTrend = new Trend('duration_get_me_user');
const getTagsTrend = new Trend('duration_get_tags');
const canCreatePostsTrend = new Trend('duration_can_create_posts');
const canMutatePostTrend = new Trend('duration_can_mutate_post');
const postMediaTrend = new Trend('duration_post_media');
const getMediaByPostIdTrend = new Trend('duration_get_media_by_post_id');
const getMediaByMediaIdTrend = new Trend('duration_get_media_by_media_id');
const deleteMediaTrend = new Trend('duration_delete_media');
const deleteCommentTrend = new Trend('duration_delete_comment');
const deletePostTrend = new Trend('duration_delete_post');
const deleteMeTrend = new Trend('duration_delete_me');
const deleteUserTrend = new Trend('duration_delete_user');
const logoutTrend = new Trend('duration_logout');

export default function () {
    const username = crypto.randomUUID().toString();
    const password = "TestPassword1!";
    const newUsername = crypto.randomUUID().toString();
    const newNewUsername = crypto.randomUUID().toString();
    
    const createdUserId = createUser(username, password);
    updateUser(createdUserId, newUsername);
    login(newUsername, password);
    const meId = getMeAuth();
    updateMe(newNewUsername);
    const createdPostId = createPost();
    getPostById(createdPostId);
    updatePost(createdPostId);
    votePost(createdPostId);
    const createdCommentId = createComment(createdPostId);
    updateComment(createdCommentId);
    voteComment(createdCommentId);
    getPosts();
    getComments(createdPostId);
    getMeUser();
    getTags();
    canCreatePosts();
    canMutatePost(createdPostId);
    const createdMediaId = postMedia(createdPostId);
    getMediaByPostId(createdPostId);
    getMediaByMediaId(createdMediaId);
    deleteMedia(createdMediaId);
    deleteComment(createdCommentId);
    deletePost(createdPostId);
    deleteMe();
    deleteUser(meId);
    logout();
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

function updateMe(newUsername) {
    const payload = JSON.stringify({ username: newUsername });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.put(`http://host.docker.internal:8000/apis/v1/me`, payload, params);

    updateMeTrend.add(res.timings.duration);
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

function getMeAuth() {
    const res = http.get('http://host.docker.internal:8000/apis/v1/authentication/me');

    getMeAuthTrend.add(res.timings.duration);
    
    return res.json().id;
}

function createPost() {
    const payload = JSON.stringify({ title: "Test Title", description: "Test Description", tags: [] });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        }
    };

    const res = http.post('http://host.docker.internal:8000/apis/v1/posts', payload, params);

    createPostTrend.add(res.timings.duration);

    return res.json().id;
}

function getPostById(postId) {
    const res = http.get(`http://host.docker.internal:8000/apis/v1/posts/${postId}`);

    getPostByIdTrend.add(res.timings.duration);
}

function updatePost(postId) {
    const payload = JSON.stringify({ description: "New Description" });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        }
    };

    const res = http.put(`http://host.docker.internal:8000/apis/v1/posts/${postId}`, payload, params);

    updatePostTrend.add(res.timings.duration);
}

function votePost(postId) {
    const payload = JSON.stringify({ voteType: "Upvote" });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        }
    };

    const res = http.put(`http://host.docker.internal:8000/apis/v1/posts/${postId}/votes`, payload, params);

    votePostTrend.add(res.timings.duration);
}

function createComment(postId) {
    const payload = JSON.stringify({ content: "Test Content" });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        }
    };

    const res = http.post(`http://host.docker.internal:8000/apis/v1/posts/${postId}/comments`, payload, params);

    createCommentTrend.add(res.timings.duration);
    
    return res.json().id;
}

function updateComment(commentId) {
    const payload = JSON.stringify({ content: "New Content" });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        }
    };

    const res = http.put(`http://host.docker.internal:8000/apis/v1/comments/${commentId}`, payload, params);

    updateCommentTrend.add(res.timings.duration);
} 

function voteComment(commentId) {
    const payload = JSON.stringify({ voteType: "Upvote" });
    const params = {
        headers: {
            'Content-Type': 'application/json',
        }
    };

    const res = http.put(`http://host.docker.internal:8000/apis/v1/comments/${commentId}/votes`, payload, params);

    voteCommentTrend.add(res.timings.duration);
}

function getPosts() {
    const res = http.get('http://host.docker.internal:8000/apis/v1/posts');

    getPostsTrend.add(res.timings.duration);
}

function getComments(postId) {
    const res = http.get(`http://host.docker.internal:8000/apis/v1/posts/${postId}/comments`);

    getCommentsTrend.add(res.timings.duration);
}

function getMeUser() {
    const res = http.get('http://host.docker.internal:8000/apis/v1/me');

    getMeUserTrend.add(res.timings.duration);
}

function getTags() {
    const res = http.get('http://host.docker.internal:8000/apis/v1/tags');

    getTagsTrend.add(res.timings.duration);
}

function canCreatePosts() {
    const res = http.get('http://host.docker.internal:8000/apis/v1/authorization/can-create-posts');

    canCreatePostsTrend.add(res.timings.duration);
}

function canMutatePost(postId) {
    const res = http.get(`http://host.docker.internal:8000/apis/v1/authorization/can-mutate-post/${postId}`);

    canMutatePostTrend.add(res.timings.duration);
}

function postMedia(postId) {
    const payload = {
        file: http.file(new Uint8Array(1024).buffer, 'random.png', 'image/png'), // filename + mime type
        description: 'Random byte file upload'
    };

    const res = http.post(`http://host.docker.internal:8000/apis/v1/media/${postId}`, payload);

    postMediaTrend.add(res.timings.duration);
    
    return res.json().id;
}

function getMediaByPostId(postId) {
    const res = http.get(`http://host.docker.internal:8000/apis/v1/media/posts/${postId}`);

    getMediaByPostIdTrend.add(res.timings.duration);
}

function getMediaByMediaId(mediaId) {
    const res = http.get(`http://host.docker.internal:8000/apis/v1/media/${mediaId}`);

    getMediaByMediaIdTrend.add(res.timings.duration);
}

function deleteMedia(mediaId) {
    const res = http.del(`http://host.docker.internal:8000/apis/v1/media/${mediaId}`);

    deleteMediaTrend.add(res.timings.duration);
}

function deleteComment(commentId) {
    const res = http.del(`http://host.docker.internal:8000/apis/v1/comments/${commentId}`);

    deleteCommentTrend.add(res.timings.duration);
}

function deletePost(postId) {
    const res = http.del(`http://host.docker.internal:8000/apis/v1/posts/${postId}`);

    deletePostTrend.add(res.timings.duration);
}

function deleteMe() {
    const res = http.del(`http://host.docker.internal:8000/apis/v1/me`);

    deleteMeTrend.add(res.timings.duration);
}

function deleteUser(userId) {
    const res = http.del(`http://host.docker.internal:8000/apis/v1/users/${userId}`);

    deleteUserTrend.add(res.timings.duration);
}

function logout() {
    const res = http.post('http://host.docker.internal:8000/apis/v1/authentication/logout');

    logoutTrend.add(res.timings.duration);
}