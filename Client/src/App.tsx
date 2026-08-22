import { BrowserRouter, Routes, Route } from "react-router-dom"
import Login from "./Pages/Login"
import Register from "./Pages/Register"

const App = () => {
  
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/register" element={<Register />} />
        {/* <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/subscriptions" element={<Subscriptions />} />
        <Route path="/payments" element={<Payments />} /> */}
      </Routes>
    </BrowserRouter>
  )
}

export default App