export const getAccessToken = () => {

    const token = localStorage.getItem("accessToken")
    if(token == "undefined"){
        return null;
    }
    return token;
}
export const setAccessToken = (accessToken: string) => localStorage.setItem("accessToken", accessToken);
export const clearAccessToken = () => localStorage.removeItem("accessToken");