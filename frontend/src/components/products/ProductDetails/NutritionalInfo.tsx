import { IProduct } from "../../../types";

interface NutritionalInfoProps {
  product: IProduct;
}

export function NutritionalInfo({ product }: NutritionalInfoProps) {
  return (
    <div className="nutrition-info mb-4">
      <h5>Nutritional Information (per 100g)</h5>
      <div className="row">
        <div className="col-md-3">
          <div className="nutrition-item" style={{ minWidth: "120px" }}>
            <strong>Calories</strong>
            <p>{product.calories} kcal</p>
          </div>
        </div>
        <div className="col-md-3">
          <div className="nutrition-item" style={{ minWidth: "120px" }}>
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
          <div className="nutrition-item" style={{ marginLeft: "25px" }}>
            <strong>Fat:</strong>
            <p>{product.fat} g</p>
          </div>
        </div>
      </div>
    </div>
  );
}
