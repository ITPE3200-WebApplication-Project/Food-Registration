import axios from "axios";
import { IProduct } from "../types";
import { ICreateProductDTO } from "../types/dtos";

const API_BASE_URL = "http://localhost:5173/api";

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Add auth interceptor
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const productApi = {
  getAll: async (category?: string, search?: string) => {
    const params = new URLSearchParams();
    if (category) params.append("category", category);
    if (search) params.append("search", search);
    const response = await api.get(`/product?${params}`);
    return response.data;
  },

  getMyProducts: async () => {
    const response = await api.get("/product/my");
    return response.data;
  },

  create: async (product: ICreateProductDTO) => {
    const response = await api.post("/product", product);
    return response.data;
  },
};

export const producerApi = {
  getMyProducers: async () => {
    const response = await api.get("/producer");
    console.log(response.data);
    return response.data;
  },
};
