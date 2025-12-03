# Generative AI - CoPilot was used in creating this file.
import os

API_BASE_URL = os.environ.get("OURCITY_API_URL", "http://localhost:8000")
API_VERSION = "v1"
API_ENDPOINT = f"{API_BASE_URL}/apis/{API_VERSION}"
