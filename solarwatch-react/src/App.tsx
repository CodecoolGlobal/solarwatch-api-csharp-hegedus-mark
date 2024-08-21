import {createBrowserRouter, RouterProvider, Navigate} from 'react-router-dom';
import './App.css'
import {AuthProvider, useAuth} from "./contexts/AuthContext.tsx";

import {Home, Login, AdminDashboard, Main} from "./pages";


const ProtectedRoute = ({children}) => {
    const {authInfo} = useAuth();
    return authInfo.authenticated ? children : <Navigate to="/login"/>
}

const AdminRoute = ({children}) => {
    const {authInfo} = useAuth();
    return authInfo.authenticated && authInfo.userRole === "admin" ? children : <Navigate to="/login"/>
}

const router = createBrowserRouter([
    {
        path: "/",
        element: <Home/>
    },
    {
        path: "/login",
        element: <Login/>
    },
    {
        path: "/admin",
        element: <AdminRoute><AdminDashboard/></AdminRoute>
    },
    {
        path: "/main",
        element: <ProtectedRoute><Main/></ProtectedRoute>
    },
]);

function App() {
    return (
        <AuthProvider>
            <RouterProvider router={router}/>
        </AuthProvider>)
}

export default App
