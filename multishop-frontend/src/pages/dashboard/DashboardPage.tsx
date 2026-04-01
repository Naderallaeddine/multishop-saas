import { useNavigate } from 'react-router-dom'
import { authStore } from '../../store/authStore'

export default function DashboardPage() {
  const navigate = useNavigate()
  const user = authStore.getUser()

  const handleLogout = () => {
    authStore.removeUser()
    navigate('/login')
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
        <h1 className="text-lg font-semibold text-gray-800">MultiShop</h1>
        <div className="flex items-center gap-4">
          <span className="text-sm text-gray-600">
            {user?.firstName} {user?.lastName}
          </span>
          <button
            onClick={handleLogout}
            className="text-sm text-red-500 hover:text-red-600"
          >
            Logout
          </button>
        </div>
      </nav>

      <main className="max-w-5xl mx-auto px-6 py-10">
        <h2 className="text-2xl font-semibold text-gray-800 mb-1">
          Welcome, {user?.firstName}!
        </h2>
        <p className="text-gray-500 text-sm mb-8">
          Your MultiShop dashboard
        </p>

        <div className="grid grid-cols-3 gap-4">
          <div className="bg-white rounded-xl border border-gray-200 p-6">
            <p className="text-sm text-gray-500">Role</p>
            <p className="text-xl font-semibold text-gray-800 mt-1">
              {user?.role}
            </p>
          </div>
          <div className="bg-white rounded-xl border border-gray-200 p-6">
            <p className="text-sm text-gray-500">Email</p>
            <p className="text-xl font-semibold text-gray-800 mt-1">
              {user?.email}
            </p>
          </div>
          <div className="bg-white rounded-xl border border-gray-200 p-6">
            <p className="text-sm text-gray-500">Status</p>
            <p className="text-xl font-semibold text-green-600 mt-1">
              Active
            </p>
          </div>
        </div>
      </main>
    </div>
  )
}