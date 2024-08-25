import React, {ReactNode} from "react";
import {createBrowserRouter, RouterProvider, Navigate} from 'react-router-dom';

import {AuthProvider, useAuth} from "./contexts/AuthContext.tsx";
import './App.css'

import {Home, Login, AdminDashboard, Main} from "./pages";
import {Register} from "./pages/Register.tsx";


interface RouteProps {
    children: ReactNode;
}

const ProtectedRoute: React.FC<RouteProps> = ({children}) => {
    const {authInfo} = useAuth();
    return authInfo.authenticated ? children : <Navigate to="/login"/>
}

const AdminRoute: React.FC<RouteProps> = ({children}) => {
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
        path: "/register",
        element: <Register/>
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
