import { useState, useEffect } from "react";
import { useSearchParams } from "react-router";
import { productApi } from "../../../services/api";
import type { IProduct } from "../../../types";
import { getImageUrl } from "../../../services/file";

export function ProductList() {
  const [products, setProducts] = useState<IProduct[]>([]);
  const [searchParams] = useSearchParams();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        let category = searchParams.get("category");
        const search = searchParams.get("searching");
        if (category === "All") {
          category = null;
        }
        const data = await productApi.getAll(
          category || undefined,
          search || undefined
        );
        setProducts(data);
      } catch (error) {
        console.error("Failed to fetch products:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchProducts();
  }, [searchParams]);

  if (loading) return <div>Loading...</div>;

  return (
    <div className="container">
      {products.length === 0 ? (
        <div className="container-fluid mt-4">
          <div className="text-center py-5">
            <i
              className="bi bi-search text-muted"
              style={{ fontSize: "4rem" }}
            ></i>
            <h3 className="mt-3 text-muted">No Products Found</h3>
            <p className="text-muted mb-4">
              {searchParams.get("searching") != null && (
                <span>
                  No products match your search for "
                  {searchParams.get("searching")}"
                </span>
              )}
              {searchParams.get("searching") == null && (
                <span>
                  Try searching for products or browse by category above
                </span>
              )}
            </p>
            {(searchParams.get("searching") != null ||
              searchParams.get("category") != null) && (
              <a href="/" className="btn btn-primary rounded-pill px-4">
                <i className="bi bi-arrow-repeat me-2"></i>Clear Search
              </a>
            )}
          </div>
        </div>
      ) : (
        <div className="row g-4">
          {products.map((product) => (
            <div key={product.productId} className="col-12 col-sm-6 col-md-4">
              <div className="product-card">
                <img
                  className="product-image"
                  src={getImageUrl(product.imageUrl)}
                  alt={product.name}
                  style={{
                    height: "200px",
                    maxHeight: "200px",
                    width: "100%",
                    objectFit: "cover",
                  }}
                />
                <div className="product-content">
                  <h5 className="product-title">{product.name}</h5>
                  <p>{product.description}</p>
                  <div className="product-footer">
                    <img
                      src={`/images/nutri-scores/Nutri-score-${product.nutritionScore}.svg.png`}
                      alt={`Nutrition Score ${product.nutritionScore}`}
                      style={{ height: "30px" }}
                    />
                    <a
                      href={`/products/${product.productId}`}
                      className="btn btn-primary rounded-pill"
                    >
                      Read More
                    </a>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
