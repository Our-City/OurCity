# Generative AI - CoPilot was used in creating this file.
import click
import getpass
from api_client import APIClient
from session_manager import SessionManager


def handle_login(session_manager):
    if session_manager.is_logged_in():
        session = session_manager.load_session()
        current_user = session.get("username")
        click.echo(f"Already logged in as {current_user}")
        
        response = input("Do you want to login as a different user? (y/n): ").lower()
        if response != 'y':
            return False
        
        session_manager.clear_session()
    
    username = input("Username: ")
    password = getpass.getpass("Password: ")
    
    api_client = APIClient()
    success, message, cookies = api_client.login(username, password)
    
    if success:
        session_manager.save_session(cookies, username)
        click.echo(message)
        return True
    else:
        click.echo(f"Login failed: {message}")
        return False


def handle_logout(session_manager):
    if not session_manager.is_logged_in():
        click.echo("Not currently logged in")
        return False

    session = session_manager.load_session()
    cookies = session.get("cookies")
    username = session.get("username")

    api_client = APIClient()
    success, message = api_client.logout(cookies)
    
    if success:
        session_manager.clear_session()
        click.echo(f"Logged out from {username}")
    else:
        session_manager.clear_session()
        click.echo(f"Cleared local session for {username}")
        click.echo(f"Note: {message}")
    
    return True


def handle_get_post(session_manager):
    post_id = input("Enter post ID: ").strip()
    
    if not post_id:
        click.echo("Post ID cannot be empty")
        return
    
    cookies = None
    if session_manager.is_logged_in():
        session = session_manager.load_session()
        cookies = session.get("cookies")
    
    api_client = APIClient()
    success, post_data, message = api_client.get_post(post_id, cookies)
    
    if success:
        click.echo("\n" + "="*60)
        click.echo(f"Title: {post_data.get('title')}")
        click.echo("="*60)
        click.echo(f"\nAuthor: {post_data.get('authorName', 'Unknown')}")
        click.echo(f"Posted: {post_data.get('createdAt', 'Unknown')}")
        click.echo(f"\nDescription:\n{post_data.get('description', 'No description')}")
        click.echo("\n" + "="*60)
    else:
        click.echo("Post was not found")


def handle_list_posts(session_manager):
    cookies = None
    if session_manager.is_logged_in():
        session = session_manager.load_session()
        cookies = session.get("cookies")

    api_client = APIClient()
    success, posts, message = api_client.get_posts(cookies)
    
    if success:
        if not posts:
            click.echo("No posts found")
            return

        click.echo(f"\nFound {len(posts)} post(s):\n")
        for i, post in enumerate(posts, 1):
            title = post.get('title', 'Untitled')
            post_id = post.get('id', 'Unknown')
            click.echo(f"{i}. {title}")
            click.echo(f"   ID: {post_id}\n")
    else:
        click.echo(f"Error: {message}")


def handle_promote(session_manager):
    if not session_manager.is_logged_in():
        click.echo("You must be logged in to promote users")
        return

    username = input("Enter username to promote to admin: ").strip()
    
    if not username:
        click.echo("Username cannot be empty")
        return

    session = session_manager.load_session()
    cookies = session.get("cookies")

    api_client = APIClient()
    success, message = api_client.promote_user_to_admin(username, cookies)
    
    if success:
        click.echo(message)
    else:
        click.echo(f"Failed to promote user: {message}")


def show_help():
    click.echo("\nAvailable commands:")
    click.echo("  login    - Login to OurCity")
    click.echo("  logout   - Logout from OurCity")
    click.echo("  list     - List all posts")
    click.echo("  post     - Get and display a post by ID")
    click.echo("  promote  - Promote a user to admin (admin only)")
    click.echo("  help     - Show this help message")
    click.echo("  exit     - Exit the application")
    click.echo()


def main():
    session_manager = SessionManager()
    
    click.echo("Welcome to OurCity CLI")
    click.echo("Type 'help' for available commands or 'exit' to quit\n")
    
    while True:
        try:
            if session_manager.is_logged_in():
                session = session_manager.load_session()
                username = session.get("username")
                prompt = f"{username}@ourcity> "
            else:
                prompt = "ourcity> "
            
            command = input(prompt).strip().lower()
            
            if not command:
                continue
            
            if command == "exit" or command == "quit":
                click.echo("Goodbye!")
                break
            elif command == "help":
                show_help()
            elif command == "login":
                logged_in = handle_login(session_manager)
                if logged_in:
                    click.echo("Type 'help' to see available commands")
            elif command == "logout":
                logged_out = handle_logout(session_manager)
                if logged_out:
                    click.echo("Type 'exit' to quit or 'login' to login again")
            elif command == "list":
                handle_list_posts(session_manager)
            elif command == "post":
                handle_get_post(session_manager)
            elif command == "promote":
                handle_promote(session_manager)
            else:
                click.echo(f"Unknown command: {command}")
                click.echo("Type 'help' for available commands")
            
            click.echo()
            
        except KeyboardInterrupt:
            click.echo("\n\nUse 'exit' to quit")
            click.echo()
        except EOFError:
            click.echo("\nGoodbye!")
            break


if __name__ == "__main__":
    main()
