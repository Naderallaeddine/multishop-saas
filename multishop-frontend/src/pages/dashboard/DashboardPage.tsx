import { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { authStore } from '../../store/authStore'
import { startOrderHub, stopOrderHub } from '../../api/signalr'

interface Notification {
  id: string
  message: string
  time: string
}

export default function DashboardPage() {
  const navigate = useNavigate()
  const user = authStore.getUser()
  const [notifications, setNotifications] = useState<Notification[]>([])
  const [connected, setConnected] = useState(false)
  const hubRef = useRef<Awaited<ReturnType<typeof startOrderHub>>>(null)

  useEffect(() => {
    let cancelled = false

    const connect = async () => {
      // For demo use a hardcoded tenantId
      // In production this comes from the user's profile
      const tenantId = 'demo-tenant'

      try {
        const hub = await startOrderHub(tenantId)
        if (!hub) return
        if (cancelled) {
          await stopOrderHub(hub)
          return
        }

        hubRef.current = hub
        setConnected(true)

        hub.on('NewOrder', (order) => {
          setNotifications(prev => [{
            id: order.orderId,
            message: `New order placed — $${order.totalAmount.toFixed(2)} (${order.itemCount} items)`,
            time: new Date().toLocaleTimeString()
          }, ...prev])
        })

        hub.on('OrderStatusChanged', (data) => {
          setNotifications(prev => [{
            id: data.orderId,
            message: `Order ${data.orderId.slice(0, 8)}... status changed to ${data.newStatus}`,
            time: new Date().toLocaleTimeString()
          }, ...prev])
        })
      } catch (err) {
        if (!cancelled) {
          console.error('SignalR connection failed — is the API running?', err)
        }
      }
    }

    void connect()
    return () => {
      cancelled = true
      setConnected(false)
      const h = hubRef.current
      hubRef.current = null
      void stopOrderHub(h)
    }
  }, [])

  const handleLogout = () => {
    authStore.removeUser()
    navigate('/login')
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center">
        <h1 className="text-lg font-semibold text-gray-800">MultiShop</h1>
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2">
            <div className={`w-2 h-2 rounded-full ${connected ? 'bg-green-500' : 'bg-gray-300'}`}/>
            <span className="text-xs text-gray-500">
              {connected ? 'Live' : 'Connecting...'}
            </span>
          </div>
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
        <p className="text-gray-500 text-sm mb-8">Your MultiShop dashboard</p>

        <div className="grid grid-cols-3 gap-4 mb-8">
          <div className="bg-white rounded-xl border border-gray-200 p-6">
            <p className="text-sm text-gray-500">Role</p>
            <p className="text-xl font-semibold text-gray-800 mt-1">{user?.role}</p>
          </div>
          <div className="bg-white rounded-xl border border-gray-200 p-6">
            <p className="text-sm text-gray-500">Email</p>
            <p className="text-lg font-semibold text-gray-800 mt-1 truncate">{user?.email}</p>
          </div>
          <div className="bg-white rounded-xl border border-gray-200 p-6">
            <p className="text-sm text-gray-500">Status</p>
            <p className="text-xl font-semibold text-green-600 mt-1">Active</p>
          </div>
        </div>

        <div className="bg-white rounded-xl border border-gray-200 p-6">
          <div className="flex justify-between items-center mb-4">
            <h3 className="text-base font-semibold text-gray-800">
              Live order notifications
            </h3>
            {notifications.length > 0 && (
              <button
                onClick={() => setNotifications([])}
                className="text-xs text-gray-400 hover:text-gray-600"
              >
                Clear all
              </button>
            )}
          </div>

          {notifications.length === 0 ? (
            <div className="text-center py-10">
              <p className="text-gray-400 text-sm">
                Waiting for orders...
              </p>
              <p className="text-gray-300 text-xs mt-1">
                New orders will appear here instantly
              </p>
            </div>
          ) : (
            <div className="space-y-3">
              {notifications.map((n, i) => (
                <div
                  key={i}
                  className="flex justify-between items-center bg-blue-50 border border-blue-100 rounded-lg px-4 py-3"
                >
                  <p className="text-sm text-blue-800">{n.message}</p>
                  <span className="text-xs text-blue-400 ml-4 shrink-0">{n.time}</span>
                </div>
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  )
}