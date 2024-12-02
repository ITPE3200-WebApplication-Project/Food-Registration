import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { IProducer, type IProduct } from "../../../types";
import { producerApi, productApi } from "../../../services/api";
import { ICreateProductDTO } from "../../../types/dtos";
import { getBase64 } from "../../../services/file";
import { AxiosError } from "axios";
import { ProductBasicInfo } from "./ProductBasicInfo";
import { ProductCategoryInfo } from "./ProductCategoryInfo";
import { NutritionalInfo } from "./NutritionalInfo";

export default function CreateProductPage() {
  const [product, setProduct] = useState<
    Partial<IProduct & { imageFile: File | null }>
  >({});
  const [producers, setProducers] = useState<IProducer[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    producerApi.getMyProducers().then(setProducers);
  }, []);

  const handleProductChange = (
    updates: Partial<IProduct & { imageFile: File | null }>
  ) => {
    setProduct((prev) => ({ ...prev, ...updates }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
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
          "Please fill in all required fields. Nutrition Score is required."
        );
        return;
      }
      if (!product.imageFile) {
        alert("Please fill in all required fields. Image is required.");
        return;
      }

      let createProductDTO: ICreateProductDTO | undefined;

      await getBase64(product.imageFile, (result) => {
        const extension =
          "." +
            product.imageFile?.name.split(".")[
              product.imageFile?.name.split(".").length - 1
            ] || "";

        createProductDTO = {
          name: product.name || "",
          description: product.description || "",
          category: product.category || "",
          nutritionScore: product.nutritionScore || "",
          imageBase64: result.split(",")[1],
          imageFileExtension: extension,
          calories: product.calories,
          carbohydrates: product.carbohydrates,
          fat: product.fat,
          protein: product.protein,
          producerId: product.producer?.producerId || 0,
        };
      });

      if (!createProductDTO) {
        alert("Please fill in all required fields. Image is required.");
        return;
      }

      try {
        await productApi.create(createProductDTO);
        navigate({
          pathname: "/products/my",
          search: "?message=Product created successfully&messageType=success",
        });
      } catch (error) {
        console.error(error);
        if (error instanceof AxiosError) {
          navigate({
            search: `?message=${error.response?.data}&messageType=danger`,
          });
        } else {
          navigate({
            search: "?message=Failed to create product&messageType=danger",
          });
        }
      }

      console.log("Product created");
    }
  };

  return (
    <div className="container my-4">
      <div className="row">
        <div className="col-12">
          <h1 className="text-center mb-4">Create New Product</h1>

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
                Create Product
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
