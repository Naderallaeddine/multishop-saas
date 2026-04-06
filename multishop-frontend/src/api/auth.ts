import api from "./axios";
import { LoginDto, RegisterDto, AuthResponse } from "../types/auth";

export const login = async (dto: LoginDto): Promise<AuthResponse> => {
  const response = await api.post<AuthResponse>("/auth/login", dto);
  return response.data;
};

export const register = async (dto: RegisterDto): Promise<AuthResponse> => {
  const response = await api.post<AuthResponse>("/auth/register", dto);
  return response.data;
};
