import {getAccessToken} from "./accessTokenUtils.ts";


export const fetcher = async (url: string, options: RequestInit = {}) => {

    // Set up default headers or other common options
    const defaultHeaders: HeadersInit = {
        'Content-Type': 'application/json',
        // Add other headers if needed
    };

    // Check for authentication token or other dynamic headers
    const token = getAccessToken();
    if (token) {
        defaultHeaders['Authorization'] = `Bearer ${token}`;
    }

    // Merge default headers with any provided options
    const config: RequestInit = {
        ...options,
        headers: {
            ...defaultHeaders,
            ...(options.headers || {}),
        },
    };

    try {
        const response = await fetch(url, config);

        if (!response.ok) {
            // Handle HTTP errors (e.g., 404, 500)
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        // Parse JSON response if content type is JSON
        const contentType = response.headers.get('Content-Type');
        if (contentType && contentType.includes('application/json')) {
            return await response.json();
        }

        // Return the response as-is if not JSON
        return await response.text();
    } catch (error) {
        // Handle fetch errors or network issues
        console.error('Fetch error:', error);
        throw error;
    }
};
