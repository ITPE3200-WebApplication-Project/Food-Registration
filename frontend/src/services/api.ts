import axios from "axios";
import {
  ICreateProducerDTO,
  ICreateProductDTO,
  IUpdateProducerDTO,
  IUpdateProductDTO,
} from "../types/dtos";

export const BASE_URL = "http://localhost:5173";

export const api = axios.create({
  baseURL: BASE_URL + "/api",
  headers: {
    "Content-Type": "application/json",
  },
});

// Add auth interceptor
// to apply the token to all requests
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
  get: async (productId: string) => {
    const response = await api.get(`/product/${productId}`);
    return response.data;
  },

  create: async (product: ICreateProductDTO) => {
    const response = await api.post("/product", product);
    return response.data;
  },

  update: async (productId: string, product: IUpdateProductDTO) => {
    const response = await api.put(`/product/${productId}`, product);
    return response.data;
  },

  delete: async (productId: string) => {
    const response = await api.delete(`/product/${productId}`);
    return response.data;
  },
};

export const producerApi = {
  getMyProducers: async () => {
    const response = await api.get("/producer");
    return response.data;
  },
  get: async (producerId: string) => {
    const response = await api.get(`/producer/${producerId}`);
    return response.data;
  },
  create: async (producer: ICreateProducerDTO) => {
    const response = await api.post("/producer", producer);
    return response.data;
  },
  update: async (producerId: string, producer: IUpdateProducerDTO) => {
    const response = await api.put(`/producer/${producerId}`, producer);
    return response.data;
  },
  delete: async (producerId: string) => {
    const response = await api.delete(`/producer/${producerId}`);
    return response.data;
  },
};
