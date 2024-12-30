import { Search } from 'lucide-react';
import { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import Perfil from './components/Perfil';
import Productos from './components/productos';
import axios from 'axios';

import "./App.css";
import re5 from './assets/images/re5.jpg';
import re6 from './assets/images/re6.jpg';
import re7 from './assets/images/re7.jpg';
import re3 from './assets/images/re3.png';
import cell from './assets/images/cell.png';
import logoss from './assets/images/rujalogo.png';

// Componente para la página de inicio
const HomePage = ({ isLoggedIn, services }) => {
    return (
        <>
            {/* Hero Section */}
            <section className="w-full bg-gray-200 py-16 md:py-24 flex-grow">
                <div className="container mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex flex-col md:flex-row gap-8">
                        {/* Info Box */}
                        <div className="bg-[#FF5722] text-white p-8 md:w-2/5">
                            <h1 className="text-3xl font-extrabold mb-4 text-white">
                                EMPRESA DE TECNOLOGÍA DE LA INFORMACIÓN
                            </h1>
                            <h2 className="text-xl font-bold mb-6">
                                CASES / CORREAS / COVER PERSONALIZADO / AIRPODS CASE Y MÁS
                            </h2>
                            <div className="space-y-4">
                                <p className="flex items-center">
                                    📍 (Hato Mayor) c/ Duarte casi esq Mercedes
                                </p>
                                <p className="flex items-center">
                                    • Lun a Sab 9:00 am a 7:00 pm
                                </p>
                                <p className="flex items-center">
                                    • Dom 9:00 - 1:00
                                </p>
                            </div>
                            <button className="mt-8 bg-white text-[#FF5722] px-6 py-2 rounded-lg hover:bg-gray-100 transition-colors">
                                Ver Catálogo
                            </button>
                        </div>

                        {/* Image Showcase */}
                        <div className="md:w-3/5 bg-gradient-to-r from-purple-200 to-blue-200 p-8 flex items-center justify-center h-full">
                            <img
                                src={cell}
                                alt="iPhone Showcase"
                                className="w-[800px] h-auto rounded-lg shadow-lg"
                            />
                        </div>
                    </div>
                </div>
            </section>

            {/* Services Grid */}
            <section className="w-full py-0 bg-white">
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-0">
                    {services.map((service, index) => (
                        <div key={index} className="relative group overflow-hidden">
                            <img
                                src={service.image}
                                alt={`Service ${index + 1}`}
                                className="w-full h-64 object-cover transform transition-transform duration-300 group-hover:scale-110"
                            />
                            <div className="absolute inset-0 bg-gradient-to-t from-black/80 to-transparent">
                                <p className="absolute bottom-4 left-4 right-4 text-white text-sm">
                                    {service.title}
                                </p>
                            </div>
                        </div>
                    ))}
                </div>
            </section>
        </>
    );
};

// Componentes provisionales para las páginas de productos
const ScreenProtector = () => <div className="p-8">Página de Protectores de Pantalla</div>;
const Phones = () => <div className="p-8">Página de Teléfonos</div>;
const Covers = () => <div className="p-8">Página de Covers</div>;

const App = () => {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [searchQuery, setSearchQuery] = useState("");
    const [isAdmin, setIsAdmin] = useState(false);

    const setupUserSession = (userData, token) => {
        localStorage.setItem('token', token);
        localStorage.setItem('userId', userData.userId);
        localStorage.setItem('userEmail', userData.email);
        localStorage.setItem('userRoles', JSON.stringify(userData.roles));
        const isAdminUser = userData.roles.includes('Admin');
        localStorage.setItem('isAdmin', isAdminUser.toString());
        setIsLoggedIn(true);
        setIsAdmin(isAdminUser);
    };

    useEffect(() => {
        const token = localStorage.getItem('token');
        const storedIsAdmin = localStorage.getItem('isAdmin') === 'true';
        if (token) {
            verifyToken(token);
        } else {
            setIsLoggedIn(false);
            setIsAdmin(false);
        }
    }, []);

    const verifyToken = async (token) => {
        try {
            const response = await axios.get(
                "https://localhost:7191/api/Register/current-user",
                {
                    headers: { 'Authorization': `Bearer ${token}` }
                }
            );
            if (response.status === 200) {
                const userData = response.data;
                console.log("Usuario verificado:", {
                    email: userData.email,
                    roles: userData.roles,
                    userId: userData.userId
                });
                setupUserSession(userData, token);
            }
        } catch (error) {
            console.error("Error de verificación del token:", error);
            handleLogout();
        }
    };


    // Simulamos obtener el rol del usuario desde el localStorage
    //const userRole = localStorage.getItem('role');

    const services = [
        {
            title: "La mejor reparación la tenemos aquí en Ruja.",
            image: re5
        },
        {
            title: "Reparamos tu celular como nuevo, rápido y seguro.",
            image: re6
        },
        {
            title: "Expertos en reparación y cuidado de tu celular.",
            image: re7
        },
        {
            title: "La mejor solución para tu teléfono, garantizada.",
            image: re3
        }
    ];

    const handleLogout = () => {
        setIsLoggedIn(false);
        setIsAdmin(false);
        localStorage.removeItem('token');
        localStorage.removeItem('isAdmin');
        localStorage.removeItem('userId');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('userRoles');
    };

    const handleSearch = (e) => {
        setSearchQuery(e.target.value);
    };

    return (
        <Router>
            <div className="min-h-screen w-full bg-gray-100 flex flex-col">
                {/* Header */}
                <header className="w-full bg-white shadow-sm sticky top-0 z-50">
                    <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-4">
                        <div className="flex items-center justify-between">
                            <div>
                                <Link to="/">
                                    <img
                                        src={logoss}
                                        alt="RUJA Logo"
                                        style={{height: '70px', width: 'auto', marginLeft: '-40px'}}
                                    />
                                </Link>
                            </div>

                            <nav className="flex items-center space-x-8 ml-auto">
                                {!isLoggedIn ? (
                                    <>
                                        <Link to="/login"
                                              className="text-[#FF5722] hover:text-orange-600 transition-colors">
                                            Iniciar sesión
                                        </Link>
                                        <Link to="/register"
                                              className="text-[#FF5722] hover:text-orange-600 transition-colors">
                                            Registrarse
                                        </Link>
                                    </>
                                ) : (
                                    <>
                                        <Link
                                            to="/productos"
                                            className="text-[#FF5722] hover:text-orange-600 transition-colors"
                                        >
                                            Productos
                                        </Link>
                                        <Link to="/protector"
                                              className="text-[#FF5722] hover:text-orange-600 transition-colors">
                                            Protector de pantalla
                                        </Link>
                                        <Link to="/telefonos"
                                              className="text-[#FF5722] hover:text-orange-600 transition-colors">
                                            Teléfonos
                                        </Link>
                                        <Link to="/covers"
                                              className="text-[#FF5722] hover:text-orange-600 transition-colors">
                                            Covers
                                        </Link>
                                        <div className="relative flex items-center space-x-4">
                                            {/* Barra de búsqueda */}
                                            <input
                                                type="text"
                                                placeholder="Buscar productos..."
                                                value={searchQuery}
                                                onChange={handleSearch}
                                                className="pl-4 pr-10 py-2 rounded-lg border border-gray-300 focus:outline-none focus:border-[#FF5722] focus:ring-1 focus:ring-[#FF5722] w-64 md:w-80"
                                            />
                                            <Search
                                                className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400"
                                                size={20}
                                            />
                                        </div>
                                    </>
                                )}
                            </nav>

                            {/* Botón de Perfil */}
                            {isLoggedIn && (
                                <div className="absolute top-10 right-9">
                                    <Link
                                        to="/perfil"
                                        className="bg-red-600 text-white hover:bg-red-700 transition-colors p-2 rounded-full"
                                    >
                                        Perfil
                                    </Link>
                                </div>
                            )}
                        </div>
                    </div>
                </header>

                {/* Routes */}
                <Routes>
                    <Route path="/" element={<HomePage isLoggedIn={isLoggedIn} services={services}/>}/>
                    <Route path="/login" element={<Login setIsLoggedIn={setIsLoggedIn}/>}/>
                    <Route path="/register" element={<Register/>}/>
                    <Route path="/productos" element={<Productos/>}/>
                    <Route path="/protector" element={<ScreenProtector/>}/>
                    <Route path="/telefonos" element={<Phones/>}/>
                    <Route path="/covers" element={<Covers/>}/>
                    <Route path="/perfil" element={<Perfil/>}/>
                </Routes>
            </div>
        </Router>
    );
};

export default App;

