import { useState, useEffect } from "react";
import type { IProducer, IProduct } from "../../../types";
import { producerApi, productApi } from "../../../services/api";
import { useNavigate, useParams } from "react-router";

import { ProductBasicInfo } from "./ProductBasicInfo";
import { ProductCategoryInfo } from "./ProductCategoryInfo";
import { NutritionalInfo } from "./NutritionalInfo";
import { AxiosError } from "axios";
import { IUpdateProductDTO } from "../../../types/dtos";
import { getBase64 } from "../../../services/file";

export default function UpdateProductPage() {
  const [product, setProduct] = useState<
    Partial<IProduct & { imageFile: File | null }>
  >({});
  const [producers, setProducers] = useState<IProducer[]>([]);
  const { productId } = useParams();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchProducers = async () => {
      const data = await producerApi.getMyProducers();
      setProducers(data);
    };
    fetchProducers();
  }, []);

  useEffect(() => {
    const fetchProduct = async () => {
      if (!productId) return;
      try {
        const data = await productApi.get(productId);
        setProduct(data);
      } catch (error) {
        console.error("Failed to fetch product:", error);
      }
    };
    fetchProduct();
  }, [productId]);

  const handleProductChange = (
    updates: Partial<IProduct & { imageFile: File | null }>
  ) => {
    setProduct((prev) => ({ ...prev, ...updates }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!productId) return;

    // Validation logic remains the same...

    // Update product logic remains the same...
    console.log(product);

    if (product) {
      if (!product.name) {
        alert("Please fill in all required fields. Name is required.");
        return;
      }
      if (!product.description) {
        alert("Please fill in all required fields. Description is required.");
        return;
      }
      if (!product.producer) {
        alert("Please fill in all required fields. Producer is required.");
        return;
      }
      if (!product.category) {
        alert("Please fill in all required fields. Category is required.");
        return;
      }
      if (!product.nutritionScore) {
        alert(
          "Please fill in all required fields. Nutrition score is required."
        );
        return;
      }

      let updateProductDTO: IUpdateProductDTO = {
        productId: parseInt(productId),
        name: product.name,
        description: product.description,
        category: product.category,
        nutritionScore: product.nutritionScore,
        producerId: product.producer.producerId,
        calories: product.calories || 0,
        carbohydrates: product.carbohydrates || 0,
        fat: product.fat || 0,
        protein: product.protein || 0,
      };

      // Get the base64 of the image
      if (product.imageFile) {
        await getBase64(product.imageFile, (result: string) => {
          const extension =
            "." +
              product.imageFile?.name.split(".")[
                product.imageFile?.name.split(".").length - 1
              ] || "";

          updateProductDTO = {
            ...updateProductDTO,
            imageBase64: result.split(",")[1],
            imageFileExtension: extension,
          };
        });
      }

      if (!updateProductDTO) {
        alert("Please fill in all required fields. Image is required.");
        return;
      }

      try {
        // Update the product
        await productApi.update(productId, updateProductDTO);
        navigate({
          pathname: "/products/my",
          search: `?message=Product updated successfully&messageType=success&productId=${productId}`,
        });
      } catch (error) {
        console.error(error);
        if (error instanceof AxiosError) {
          navigate({
            search: `?message=${error.response?.data}&messageType=danger`,
          });
        } else {
          navigate({
            search: "?message=Failed to update product&messageType=danger",
          });
        }
      }
    }
  };

  if (!product) return <div>Loading...</div>;

  return (
    <div className="container my-4">
      <div className="row justify-content-center align-items-center">
        <div className="col-12 col-lg-10 col-xl-8">
          <h1 className="text-center mb-4">Update Product</h1>

          <form onSubmit={handleSubmit}>
            <ProductBasicInfo
              product={product}
              producers={producers}
              onProductChange={handleProductChange}
            />

            <ProductCategoryInfo
              product={product}
              onProductChange={handleProductChange}
            />

            <NutritionalInfo
              product={product}
              onProductChange={handleProductChange}
            />

            <div className="form-group mt-4">
              <button type="submit" className="btn btn-success">
                Save Changes
              </button>
              <a href="/" className="btn btn-secondary ms-2">
                Cancel
              </a>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
