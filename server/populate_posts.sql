-- Script to populate database with posts across the last year
-- Run this script against your PostgreSQL database

-- First, ensure we have at least one user to assign as author
-- If no users exist, create a demo user (adjust if you already have users)
DO $$
DECLARE
    demo_user_id UUID;
    tag_ids UUID[] := ARRAY[
        '3b84d6d5-4d4e-4e09-8a90-6c2d257ae14c'::UUID, -- Construction
        '6b8e5470-5a3e-48a7-a3e3-142e7e8b2e02'::UUID, -- Transportation
        '5f8b0e26-33a1-4e9f-a3c5-7e78f32f804a'::UUID, -- Entertainment
        '91f75b8d-bf32-46af-a6a9-8f89417cbbd0'::UUID, -- Shopping
        'c4db2614-0d47-4c16-89da-fd8c97a216f4'::UUID, -- Food & Dining
        '8c7b9a39-b4a9-40a3-85ce-034d97a2a6c2'::UUID, -- Parks & Recreation
        'f1f8e911-61db-45a2-b9df-7dc6de4c9a0d'::UUID, -- Safety
        '1f59e1d4-37b7-4ad2-9f6f-431a5e8cf8b7'::UUID, -- Community Events
        '41a6f4ac-8a91-4209-b40e-8b14b9a01873'::UUID, -- Infrastructure
        '4f6329f1-3201-4a94-b41c-cf74ed91f777'::UUID  -- Business
    ];
    post_id UUID;
    created_date TIMESTAMP;
    num_posts INTEGER := 365; -- Approximately one post per day over the year
    i INTEGER;
    random_tag_count INTEGER;
    j INTEGER;
BEGIN
    -- Get the first user from the database (or use a specific user ID)
    SELECT "Id" INTO demo_user_id FROM "AspNetUsers" LIMIT 1;
    
    -- If no user exists, you'll need to create one first or adjust this query
    IF demo_user_id IS NULL THEN
        RAISE NOTICE 'No users found in database. Please create a user first.';
        RETURN;
    END IF;

    RAISE NOTICE 'Using user ID: %', demo_user_id;

    -- Generate posts distributed over the last year
    FOR i IN 1..num_posts LOOP
        post_id := gen_random_uuid();
        
        -- Distribute posts randomly over the last 365 days
        created_date := NOW() - (RANDOM() * 365 || ' days')::INTERVAL;
        
        -- Insert post
        INSERT INTO "Posts" (
            "Id",
            "AuthorId",
            "Title",
            "Description",
            "Location",
            "Latitude",
            "Longitude",
            "Visisbility",
            "IsDeleted",
            "CreatedAt",
            "UpdatedAt"
        ) VALUES (
            post_id,
            demo_user_id,
            'Post ' || i || ': ' || (ARRAY['New Development Project', 'Road Maintenance Needed', 'Community Event', 'Park Improvement Idea', 'Local Business Opening', 'Safety Concern', 'Traffic Issue', 'Recreation Facility Request', 'Food Festival Announcement', 'Infrastructure Update'])[1 + floor(random() * 10)],
            'This is a sample post created to populate analytics data. Post number ' || i || ' was created on ' || created_date::DATE || '.',
            (ARRAY['Downtown', 'North End', 'South Side', 'East District', 'West End', 'City Center', 'Suburbs', 'Waterfront'])[1 + floor(random() * 8)],
            49.8951 + (RANDOM() * 0.1 - 0.05), -- Random latitude around Winnipeg
            -97.1384 + (RANDOM() * 0.1 - 0.05), -- Random longitude around Winnipeg
            0, -- Published
            false,
            created_date,
            created_date
        );
        
        -- Add 1-3 random tags to each post
        random_tag_count := 1 + floor(random() * 3)::INTEGER;
        
        FOR j IN 1..random_tag_count LOOP
            -- Insert into PostTag junction table
            INSERT INTO "PostTag" ("PostsId", "TagsId")
            VALUES (post_id, tag_ids[1 + floor(random() * array_length(tag_ids, 1))])
            ON CONFLICT DO NOTHING; -- Avoid duplicate tag assignments
        END LOOP;
        
        -- Randomly add some votes to posts (60% chance of having votes)
        IF RANDOM() < 0.6 THEN
            -- Add 1-10 upvotes
            FOR j IN 1..floor(1 + random() * 10)::INTEGER LOOP
                INSERT INTO "PostVotes" (
                    "Id",
                    "PostId",
                    "VoterId",
                    "VoteType",
                    "VotedAt"
                ) VALUES (
                    gen_random_uuid(),
                    post_id,
                    demo_user_id,
                    1, -- Upvote
                    created_date + (RANDOM() * INTERVAL '24 hours')
                );
            END LOOP;
        END IF;
        
        -- Randomly add some downvotes (20% chance)
        IF RANDOM() < 0.2 THEN
            FOR j IN 1..floor(1 + random() * 3)::INTEGER LOOP
                INSERT INTO "PostVotes" (
                    "Id",
                    "PostId",
                    "VoterId",
                    "VoteType",
                    "VotedAt"
                ) VALUES (
                    gen_random_uuid(),
                    post_id,
                    demo_user_id,
                    -1, -- Downvote
                    created_date + (RANDOM() * INTERVAL '24 hours')
                );
            END LOOP;
        END IF;
        
        -- Randomly add some comments (40% chance)
        IF RANDOM() < 0.4 THEN
            FOR j IN 1..floor(1 + random() * 5)::INTEGER LOOP
                INSERT INTO "Comments" (
                    "Id",
                    "PostId",
                    "AuthorId",
                    "Content",
                    "IsDeleted",
                    "CreatedAt",
                    "UpdatedAt"
                ) VALUES (
                    gen_random_uuid(),
                    post_id,
                    demo_user_id,
                    'Sample comment ' || j || ' on this post.',
                    false,
                    created_date + (RANDOM() * INTERVAL '48 hours'),
                    created_date + (RANDOM() * INTERVAL '48 hours')
                );
            END LOOP;
        END IF;
        
        -- Progress indicator every 50 posts
        IF i % 50 = 0 THEN
            RAISE NOTICE 'Created % posts...', i;
        END IF;
    END LOOP;

    RAISE NOTICE 'Successfully created % posts with tags, votes, and comments!', num_posts;
END $$;
