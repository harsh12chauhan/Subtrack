import { Link, useLocation } from "react-router-dom";
import { logout } from "../Utils/Logout";

const Sidebar = () => {
    
  const location = useLocation();

  const menuItems = [
    {
      name: "Dashboard",
      path: "/dashboard",
    },
    {
      name: "Subscriptions",
      path: "/subscriptions",
    },
    {
      name: "Payments",
      path: "/payments",
    },
    {
      name: "Budget",
      path: "/budget",
    },
    {
      name: "Notifications",
      path: "/notifications",
    },
  ];

  return (
    <aside className="w-64 h-screen bg-slate-900 text-white p-5">
      <h1 className="text-2xl font-bold mb-10">
        SubTrack
      </h1>

      <nav>
        <ul className="space-y-3">
          {menuItems.map((item) => (
            <li key={item.path}>
              <Link
                to={item.path}
                className={`block p-3 rounded-lg transition ${
                  location.pathname === item.path
                    ? "bg-blue-600"
                    : "hover:bg-slate-800"
                }`}
              >
                {item.name}
              </Link>
            </li>
          ))}
        </ul>
      </nav>

      <div className="absolute bottom-5 left-5">
        <button
          className="bg-red-600 hover:bg-red-700 px-4 py-2 rounded-lg"
          onClick={logout}
        >
          Logout
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
