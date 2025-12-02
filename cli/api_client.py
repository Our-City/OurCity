# Generative AI - CoPilot was used in creating this file.
import requests
from typing import Optional, Tuple
from config import API_ENDPOINT


class APIClient:

    def __init__(self):
        self.base_url = API_ENDPOINT
        self.session = requests.Session()

    def login(self, username: str, password: str) -> Tuple[bool, str, Optional[dict]]:
        url = f"{self.base_url}/authentication/login"
        payload = {
            "username": username,
            "password": password
        }

        try:
            response = self.session.post(url, json=payload)
            
            if response.status_code == 204:
                # Successful login
                cookies = self.session.cookies.get_dict()
                return True, f"Successfully logged in as {username}", cookies
            elif response.status_code == 401:
                return False, "Invalid credentials", None
            else:
                return False, f"Login failed with status code {response.status_code}", None
                
        except requests.exceptions.ConnectionError:
            return False, "Could not connect to the server. Is it running?", None
        except Exception as e:
            return False, f"An error occurred: {str(e)}", None

    def logout(self, cookies: dict) -> Tuple[bool, str]:
        url = f"{self.base_url}/authentication/logout"
        
        try:
            # Set cookies for this request
            response = self.session.post(url, cookies=cookies)
            
            if response.status_code == 204:
                return True, "Successfully logged out"
            elif response.status_code == 401:
                return False, "Not authenticated or session expired"
            else:
                return False, f"Logout failed with status code {response.status_code}"
                
        except requests.exceptions.ConnectionError:
            return False, "Could not connect to the server. Is it running?"
        except Exception as e:
            return False, f"An error occurred: {str(e)}"

    def get_current_user(self, cookies: dict) -> Tuple[bool, Optional[dict], str]:
        url = f"{self.base_url}/authentication/me"
        
        try:
            response = self.session.get(url, cookies=cookies)
            
            if response.status_code == 200:
                user_data = response.json()
                return True, user_data, "User information retrieved successfully"
            elif response.status_code == 401:
                return False, None, "Not authenticated or session expired"
            else:
                return False, None, f"Request failed with status code {response.status_code}"
                
        except requests.exceptions.ConnectionError:
            return False, None, "Could not connect to the server. Is it running?"
        except Exception as e:
            return False, None, f"An error occurred: {str(e)}"

    def get_post(self, post_id: str, cookies: Optional[dict] = None) -> Tuple[bool, Optional[dict], str]:
        url = f"{self.base_url}/posts/{post_id}"
        
        try:
            if cookies:
                response = self.session.get(url, cookies=cookies)
            else:
                response = self.session.get(url)
            
            if response.status_code == 200:
                post_data = response.json()
                return True, post_data, "Post retrieved successfully"
            elif response.status_code == 404:
                return False, None, "Post not found"
            else:
                return False, None, f"Request failed with status code {response.status_code}"
                
        except requests.exceptions.ConnectionError:
            return False, None, "Could not connect to the server. Is it running?"
        except Exception as e:
            return False, None, f"An error occurred: {str(e)}"

    def get_posts(self, cookies: Optional[dict] = None) -> Tuple[bool, Optional[list], str]:
        url = f"{self.base_url}/posts"
        
        try:
            if cookies:
                response = self.session.get(url, cookies=cookies)
            else:
                response = self.session.get(url)
            
            if response.status_code == 200:
                data = response.json()
                posts = data.get('items', [])
                return True, posts, "Posts retrieved successfully"
            else:
                return False, None, f"Request failed with status code {response.status_code}"
                
        except requests.exceptions.ConnectionError:
            return False, None, "Could not connect to the server. Is it running?"
        except Exception as e:
            return False, None, f"An error occurred: {str(e)}"

    def promote_user_to_admin(self, username: str, cookies: dict) -> Tuple[bool, str]:
        url = f"{self.base_url}/admin/users/{username}/promote-to-admin"
        
        try:
            session = requests.Session()
            for key, value in cookies.items():
                session.cookies.set(key, value)
            
            response = session.put(url)
            
            if response.status_code == 204:
                return True, f"Successfully promoted {username} to admin"
            elif response.status_code == 401:
                return False, "Not authenticated or session expired"
            elif response.status_code == 403:
                return False, "You do not have permission to promote users"
            elif response.status_code == 404:
                return False, "User not found"
            else:
                return False, f"Request failed with status code {response.status_code}"
                
        except requests.exceptions.ConnectionError:
            return False, "Could not connect to the server. Is it running?"
        except Exception as e:
            return False, f"An error occurred: {str(e)}"
