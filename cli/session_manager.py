# Generative AI - CoPilot was used in creating this file.
from typing import Optional

class SessionManager:
    def __init__(self):
        self.session_data = None

    def save_session(self, cookies: dict, username: str) -> None:
        self.session_data = {
            "cookies": cookies,
            "username": username
        }

    def load_session(self) -> Optional[dict]:
        return self.session_data

    def clear_session(self) -> None:
        self.session_data = None

    def is_logged_in(self) -> bool:
        return self.session_data is not None and "cookies" in self.session_data
