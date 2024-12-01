import type { IProduct } from "../../types";

interface ReadMoreProps {
  product: IProduct;
}

export default function ReadMore({ product }: ReadMoreProps) {
  return (
    <div className="container my-4">
      <div className="card shadow">
        <div className="row g-0">
          <div className="col-md-4">
            <img
              src={product.imageUrl}
              className="img-fluid rounded-start h-100"
              alt={product.name}
              style={{ objectFit: "cover" }}
            />
          </div>
          <div className="col-md-8">
            <div className="card-body">
              <h2 className="card-title mb-4">{product.name}</h2>

              <div className="mb-3">
                <h5>Product Details</h5>
                <p>{product.description}</p>
              </div>

              <div className="mb-3">
                <h5>Category</h5>
                <p>{product.category}</p>
              </div>

              <div className="mb-3">
                <h5>Producer</h5>
                <p>{product.producer}</p>
              </div>

              <div className="mb-4">
                <h5>Nutritional Information (per 100g)</h5>
                <div className="row">
                  <div className="col-md-3">
                    <p className="mb-1">Calories:</p>
                    <strong>{product.calories} kcal</strong>
                  </div>
                  <div className="col-md-3">
                    <p className="mb-1">Protein:</p>
                    <strong>{product.protein}g</strong>
                  </div>
                  <div className="col-md-3">
                    <p className="mb-1">Carbohydrates:</p>
                    <strong>{product.carbohydrates}g</strong>
                  </div>
                  <div className="col-md-3">
                    <p className="mb-1">Fat:</p>
                    <strong>{product.fat}g</strong>
                  </div>
                </div>
              </div>

              <div className="mb-3">
                <h5>Nutrition Score</h5>
                <p>{product.nutritionScore}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
