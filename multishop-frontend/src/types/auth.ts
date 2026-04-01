export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  expiresAt: string;
}

export interface User {
  token: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
}
