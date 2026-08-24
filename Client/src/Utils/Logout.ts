import { useNavigate } from "react-router-dom";

  const navigate = useNavigate();

  export const logout = () => {
      
    localStorage.removeItem("accessToken");
    navigate("/");
    
  }