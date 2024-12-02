import { IProduct } from "../../../types";
import { NUTRITION_SCORES } from "../../shared/constants";

interface NutritionalInfoProps {
  product: Partial<IProduct & { imageFile: File | null }>;
  onProductChange: (
    updates: Partial<IProduct & { imageFile: File | null }>
  ) => void;
}

export function NutritionalInfo({
  product,
  onProductChange,
}: NutritionalInfoProps) {
  return (
    <>
      <div className="row">
        <div className="col-md-3">
          <div className="form-group mb-3">
            <label className="form-label">Calories (kcal)</label>
            <input
              type="number"
              className="form-control"
              min="0"
              step="1"
              value={product.calories}
              onChange={(e) =>
                onProductChange({
                  calories: parseFloat(e.target.value),
                })
              }
            />
          </div>
        </div>
        <div className="col-md-3">
          <div className="form-group mb-3">
            <label className="form-label">Protein (g)</label>
            <input
              type="number"
              className="form-control"
              min="0"
              step="1"
              value={product.protein}
              onChange={(e) =>
                onProductChange({
                  protein: parseFloat(e.target.value),
                })
              }
            />
          </div>
        </div>
        <div className="col-md-3">
          <div className="form-group mb-3">
            <label className="form-label">Carbohydrates (g)</label>
            <input
              type="number"
              className="form-control"
              min="0"
              step="1"
              value={product.carbohydrates}
              onChange={(e) =>
                onProductChange({
                  carbohydrates: parseFloat(e.target.value),
                })
              }
            />
          </div>
        </div>
        <div className="col-md-3">
          <div className="form-group mb-3">
            <label className="form-label">Fat (g)</label>
            <input
              type="number"
              className="form-control"
              min="0"
              step="1"
              value={product.fat}
              onChange={(e) =>
                onProductChange({
                  fat: parseFloat(e.target.value),
                })
              }
            />
          </div>
        </div>
      </div>

      <div className="form-group mb-3">
        <label className="form-label">
          Nutrition Score<span className="text-danger">*</span>
        </label>
        <select
          className="form-select"
          required
          value={product.nutritionScore}
          onChange={(e) =>
            onProductChange({
              nutritionScore: e.target.value,
            })
          }
        >
          {NUTRITION_SCORES.map((score) => (
            <option key={score} value={score}>
              {score}
            </option>
          ))}
        </select>
      </div>
    </>
  );
}
