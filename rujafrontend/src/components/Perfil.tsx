import { useEffect, useState } from "react";
import axios from "axios";

const Profile = () => {
    const [user, setUser] = useState(null);
    const [error, setError] = useState(null);

    const handleLogout = () => {
        localStorage.removeItem("token");
        window.location.href = "/login";
    };

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const token = localStorage.getItem("token"); // Recuperamos el token desde el almacenamiento local.
                if (!token) {
                    throw new Error("Usuario no autenticado");
                }

                const response = await axios.get("https://localhost:7191/api/Register/current-user", {
                    headers: {
                        Authorization: `Bearer ${token}`, // Incluimos el token en los encabezados.
                    },
                });
                setUser(response.data);
            } catch (err) {
                setError(err.message || "Error al obtener los datos del usuario");
            }
        };

        fetchUserData();
    }, []);

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold mb-4">Perfil</h1>
            {error ? (
                <p className="text-red-600">{error}</p>
            ) : user ? (
                <div>

                    <p><strong>Email:</strong> {user.email}</p>
                    <p><strong>Roles:</strong> {user.roles.join(", ")}</p>
                </div>
            ) : (
                <p>Cargando información del usuario...</p>
            )}

            <button
                onClick={handleLogout}
                className="mt-8 bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded transition-colors"
            >
                Cerrar Sesión
            </button>
        </div>
    );
};

export default Profile;
