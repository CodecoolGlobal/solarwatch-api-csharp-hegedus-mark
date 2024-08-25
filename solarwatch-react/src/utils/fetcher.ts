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
        const contentType = response.headers.get('Content-Type');
        const isJson = contentType && contentType.includes('application/json');


        if (!response.ok) {
            // Try to get the error message from the body
            let errorMessage = `HTTP error! Status: ${response.status}`;
            if (isJson) {
                const errorBody = await response.json();
                errorMessage += ` - ${errorBody.message || JSON.stringify(errorBody)}`;
            } else {
                const errorText = await response.text();
                errorMessage += ` - ${errorText}`;
            }
            throw new Error(errorMessage);
        }

        return isJson ? await response.json() : await response.text();
    } catch (error) {
        // Handle fetch errors or network issues
        console.error('Fetch error:', error);
        throw error;
    }
};
