import { Navigate } from 'react-router-dom'
import { authStore } from '../store/authStore'

export default function ProtectedRoute({ children }: { children: React.ReactNode }) {
  if (!authStore.isLoggedIn()) {
    return <Navigate to="/login" replace />
  }
  return <>{children}</>
}