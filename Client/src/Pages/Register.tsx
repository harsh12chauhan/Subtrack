import React from 'react'

const Register = () => {
  return (
    <div>Register</div>
  )
}

export default Register

// import { useState } from "react";
// import { useNavigate } from "react-router-dom";
// import { registerUser } from "../services/authService";

// const Register = () => {
//   const navigate = useNavigate();

//   const [username, setUsername] = useState("");
//   const [password, setPassword] = useState("");
//   const [role, setRole] = useState("User");

//   const [error, setError] = useState("");
//   const [successMessage, setSuccessMessage] = useState("");
//   const [isLoading, setIsLoading] = useState(false);

//   const handleRegister = async () => {
//     try {
//       setError("");
//       setSuccessMessage("");
//       setIsLoading(true);

//       await registerUser(username, password, role);

//       setSuccessMessage("Registration successful!");

//       setTimeout(() => {
//         navigate("/login");
//       }, 1500);

//     } catch (error) {
//       console.error(error);
//       setError("Registration failed");
//     } finally {
//       setIsLoading(false);
//     }
//   };

//   return (
//     <div className="min-h-screen flex items-center justify-center bg-slate-100">
//       <div className="bg-white shadow-lg rounded-xl p-8 w-full max-w-md">

//         <h1 className="text-3xl font-bold text-center mb-6">
//           Register
//         </h1>

//         <div className="space-y-4">

//           <div>
//             <label className="block mb-1">
//               Username
//             </label>

//             <input
//               type="text"
//               value={username}
//               onChange={(e) => setUsername(e.target.value)}
//               placeholder="Enter username"
//               className="w-full border rounded-lg p-3"
//             />
//           </div>

//           <div>
//             <label className="block mb-1">
//               Password
//             </label>

//             <input
//               type="password"
//               value={password}
//               onChange={(e) => setPassword(e.target.value)}
//               placeholder="Enter password"
//               className="w-full border rounded-lg p-3"
//             />
//           </div>

//           <div>
//             <label className="block mb-1">
//               Role
//             </label>

//             <select
//               value={role}
//               onChange={(e) => setRole(e.target.value)}
//               className="w-full border rounded-lg p-3"
//             >
//               <option value="User">User</option>
//               <option value="Admin">Admin</option>
//             </select>
//           </div>

//           {error && (
//             <p className="text-red-500 text-sm">
//               {error}
//             </p>
//           )}

//           {successMessage && (
//             <p className="text-green-600 text-sm">
//               {successMessage}
//             </p>
//           )}

//           <button
//             onClick={handleRegister}
//             disabled={isLoading}
//             className="w-full bg-green-600 text-white p-3 rounded-lg hover:bg-green-700"
//           >
//             {isLoading ? "Registering..." : "Register"}
//           </button>

//         </div>
//       </div>
//     </div>
//   );
// };

// export default Register;