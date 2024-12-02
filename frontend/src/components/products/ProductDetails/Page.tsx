import { Link, useParams } from "react-router";
import type { IProduct } from "../../../types";
import { useEffect, useState } from "react";
import { productApi } from "../../../services/api";
import { getImageUrl } from "../../../services/file";
import { ProducerInfo } from "./ProducerInfo";
import { ProductInfo } from "./ProductInfo";
import { NutritionalInfo } from "./NutritionalInfo";

export default function ProductDetailsPage() {
  const [product, setProduct] = useState<IProduct>();
  const { productId } = useParams();

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

  if (!product) return <div>Loading...</div>;

  return (
    <div className="container my-4">
      <div className="card shadow">
        <div className="row g-0">
          <div className="col-md-4">
            <img
              src={getImageUrl(product.imageUrl)}
              className="img-fluid rounded-start h-100"
              alt={product.name}
              style={{ objectFit: "cover" as const }}
            />
          </div>
          <div className="col-md-8">
            <div className="card-body">
              <h2 className="card-title mb-4">{product.name}</h2>

              {product.producer && <ProducerInfo producer={product.producer} />}
              <ProductInfo product={product} />
              <NutritionalInfo product={product} />

              <div className="mt-4">
                <Link to="/" className="btn btn-primary rounded-pill">
                  <i className="bi bi-arrow-left"></i> Back to Products
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
