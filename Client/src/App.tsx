import { BrowserRouter, Routes, Route } from "react-router-dom"
import Login from "./Pages/Login"
import Register from "./Pages/Register"
import AppRoute from "./Routes/AppRoute"

const App = () => {
  
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <AppRoute/>      
      </Routes>
    </BrowserRouter>
  )
}

export default App