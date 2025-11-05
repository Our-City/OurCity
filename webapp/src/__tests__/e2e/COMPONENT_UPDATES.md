# Adding data-testid Attributes to Components

To make your e2e tests more reliable, add `data-testid` attributes to your Vue components. Here are examples:

## PostList.vue Example

```vue
<template>
  <div data-testid="post-list">
    <div 
      v-for="post in posts" 
      :key="post.id"
      data-testid="post-item"
    >
      <!-- Post content -->
    </div>
  </div>
</template>
```

## SideBar.vue Example

```vue
<template>
  <aside data-testid="sidebar">
    <nav>
      <RouterLink to="/" data-testid="nav-home">Home</RouterLink>
      <RouterLink to="/profile" data-testid="nav-profile">Profile</RouterLink>
    </nav>
  </aside>
</template>
```

## VoteBox.vue Example

```vue
<template>
  <div data-testid="vote-box">
    <button 
      @click="upvote"
      data-testid="upvote-button"
      aria-label="Upvote"
    >
      ↑
    </button>
    
    <span data-testid="vote-count">{{ voteCount }}</span>
    
    <button 
      @click="downvote"
      data-testid="downvote-button"
      aria-label="Downvote"
    >
      ↓
    </button>
  </div>
</template>
```

## CommentList.vue Example

```vue
<template>
  <div data-testid="comment-list">
    <div 
      v-for="comment in comments" 
      :key="comment.id"
      data-testid="comment-item"
    >
      <div data-testid="comment-content">{{ comment.content }}</div>
      <div data-testid="comment-author">{{ comment.author }}</div>
    </div>
    
    <form @submit.prevent="submitComment" data-testid="comment-form">
      <textarea 
        v-model="newComment"
        data-testid="comment-input"
        placeholder="Add a comment..."
      />
      <button type="submit" data-testid="comment-submit">Submit</button>
    </form>
  </div>
</template>
```

## Login/Register Forms Example

```vue
<template>
  <form @submit.prevent="handleLogin" data-testid="login-form">
    <div>
      <label for="email">Email</label>
      <input 
        id="email"
        v-model="email"
        type="email"
        data-testid="email-input"
      />
    </div>
    
    <div>
      <label for="password">Password</label>
      <input 
        id="password"
        v-model="password"
        type="password"
        data-testid="password-input"
      />
    </div>
    
    <button type="submit" data-testid="login-submit">Login</button>
  </form>
</template>
```

## Best Practices

1. **Use semantic names**: `data-testid="post-item"` not `data-testid="div1"`
2. **Be consistent**: Use kebab-case for all test IDs
3. **Add to interactive elements**: buttons, inputs, links, forms
4. **Add to container elements**: lists, cards, modals
5. **Use dynamic values when needed**: 
   ```vue
   <div :data-testid="`post-${post.id}`">
   ```

## Testing with data-testid

```typescript
// In your tests
await page.locator('[data-testid="post-list"]').waitFor();
await page.locator('[data-testid="upvote-button"]').click();
const count = await page.locator('[data-testid="vote-count"]').textContent();
```
