import { Link } from "react-router";
import type { IProduct } from "../../types";

interface ProductDetailsProps {
  product: IProduct;
}

const ProductDetails: React.FC<ProductDetailsProps> = ({ product }) => {
  return (
    <div className="container my-4">
      <div className="card shadow">
        <div className="row g-0">
          <div className="col-md-4">
            <img
              src={product.imageUrl}
              className="img-fluid rounded-start h-100"
              alt={product.name}
              style={{ objectFit: "cover" as const }}
            />
          </div>
          <div className="col-md-8">
            <div className="card-body">
              <h2 className="card-title mb-4">{product.name}</h2>

              {/* Producer Information Section */}
              {product.producer && (
                <div className="producer-info mb-4 p-3 border rounded">
                  <div className="row align-items-center">
                    <div className="col">
                      <h5 className="mb-2">Producer: {product.producer}</h5>
                    </div>
                  </div>
                </div>
              )}

              <div className="mb-4">
                <h5>Description</h5>
                <p className="card-text">{product.description}</p>
              </div>

              <div className="mb-4">
                <h5>Category</h5>
                <p className="card-text">{product.category}</p>
              </div>

              <div className="nutrition-info mb-4">
                <h5>Nutritional Information (per 100g)</h5>
                <div className="row">
                  <div className="col-md-3">
                    <div
                      className="nutrition-item"
                      style={{ minWidth: "120px" }}
                    >
                      <strong>Calories</strong>
                      <p>{product.calories} kcal</p>
                    </div>
                  </div>
                  <div className="col-md-3">
                    <div
                      className="nutrition-item"
                      style={{ minWidth: "120px" }}
                    >
                      <strong>Protein</strong>
                      <p>{product.protein} g</p>
                    </div>
                  </div>
                  <div className="col-md-3">
                    <div
                      className="nutrition-item text-nowrap"
                      style={{ width: "145px" }}
                    >
                      <strong>Carbohydrates:</strong>
                      <p>{product.carbohydrates} g</p>
                    </div>
                  </div>
                  <div className="col-md-3">
                    <div
                      className="nutrition-item"
                      style={{ marginLeft: "25px" }}
                    >
                      <strong>Fat:</strong>
                      <p>{product.fat} g</p>
                    </div>
                  </div>
                </div>
              </div>

              <div className="mt-4">
                <Link to="/products" className="btn btn-primary rounded-pill">
                  <i className="bi bi-arrow-left"></i> Back to Products
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetails;
