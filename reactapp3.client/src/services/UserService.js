
export function getUserFromToken() {
    const token = localStorage.getItem("accessToken");
    if (!token) return null;

    try {
        // Decode JWT (split by '.' and decode the payload)
        const payload = JSON.parse(atob(token.split(".")[1]));
        return {
            firstName: payload.firstName,
            lastName: payload.lastName,
            email: payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]
        };
    } catch (err) {
        console.error("Error decoding token:", err);
        return null;
    }
}

