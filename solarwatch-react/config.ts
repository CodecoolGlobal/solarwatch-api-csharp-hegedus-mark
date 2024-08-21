const PORT = 8080;

const API_BASE_URL = `http://localhost:${PORT}/api`;

export const API_ENDPOINTS = {
    LOGIN: `${API_BASE_URL}/Auth/login`,
    REGISTER: `${API_BASE_URL}/Auth/register`,
}

