import { useEffect, useState } from "react";
import type { IProduct } from "../../../types";
import DeleteModal from "../../shared/DeleteModal";
import { productApi } from "../../../services/api";
import { authService } from "../../../services/auth";
import { Navigate, useLocation, useNavigate } from "react-router";
import { AxiosError } from "axios";
import { ProductsTable } from "./ProductsTable";
import { ActionBar } from "./ActionBar";

export default function MyProductsPage() {
  const { loggedIn } = authService.getUser();
  const [products, setProducts] = useState<IProduct[]>([]);
  const navigate = useNavigate();
  const path = useLocation();

  useEffect(() => {
    console.log(path.search);
    productApi.getMyProducts().then(setProducts);
  }, [path.search]);

  const [showModal, setShowModal] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<{
    id: number;
    name: string;
  } | null>(null);

  if (!loggedIn) {
    return <Navigate to="/login" />;
  }

  const handleShowModal = (id: number, name: string) => {
    setSelectedProduct({ id, name });
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setSelectedProduct(null);
  };

  const handleConfirmDelete = async (id: number) => {
    try {
      await productApi.delete(id.toString());
      handleCloseModal();
      navigate({
        search:
          "?message=Product deleted successfully&messageType=success&productId=" +
          id,
      });
    } catch (error) {
      if (error instanceof AxiosError) {
        navigate({
          search: `?message=${error.response?.data}&messageType=danger`,
        });
      } else {
        navigate({
          search: "?message=Failed to delete product&messageType=danger",
        });
      }
    }
  };

  return (
    <div className="container my-4">
      <h1 className="text-center mb-4">My Products</h1>
      <div
        className="card shadow-sm"
        style={{
          width: "100%",
          maxWidth: "none",
          height: "100%",
          maxHeight: "none",
        }}
      >
        <div className="card-body px-4 py-2">
          <ActionBar />
          <ProductsTable products={products} onDelete={handleShowModal} />
        </div>
      </div>

      <DeleteModal
        show={showModal}
        itemId={selectedProduct?.id || null}
        itemName={selectedProduct?.name || ""}
        itemType="product"
        onClose={handleCloseModal}
        onConfirm={handleConfirmDelete}
      />
    </div>
  );
}
