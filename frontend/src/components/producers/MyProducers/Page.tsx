import { useEffect, useState } from "react";
import type { IProducer } from "../../../types";
import DeleteModal from "../../shared/DeleteModal";
import { producerApi } from "../../../services/api";
import { authService } from "../../../services/auth";
import { Navigate, useLocation, useNavigate } from "react-router";
import { AxiosError } from "axios";
import { ProducersTable } from "./ProducersTable";
import { ActionBar } from "./ActionBar";

export default function MyProducersPage() {
  const { loggedIn } = authService.getUser();
  const [producers, setProducers] = useState<IProducer[]>([]);
  const navigate = useNavigate();
  const path = useLocation();

  useEffect(() => {
    producerApi.getMyProducers().then(setProducers);
  }, [path.search]);

  const [showModal, setShowModal] = useState(false);
  const [selectedProducer, setSelectedProducer] = useState<{
    id: number;
    name: string;
  } | null>(null);

  if (!loggedIn) {
    return <Navigate to="/login" />;
  }

  const handleShowModal = (id: number, name: string) => {
    setSelectedProducer({ id, name });
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setSelectedProducer(null);
  };

  const handleConfirmDelete = async (id: number) => {
    try {
      await producerApi.delete(id.toString());
      navigate({
        search:
          "?message=Producer deleted successfully&messageType=success&producerId=" +
          id,
      });
    } catch (error) {
      if (error instanceof AxiosError) {
        navigate({
          search: `?message=${error.response?.data}&messageType=danger`,
        });
      } else {
        navigate({
          search: "?message=Failed to delete producer&messageType=danger",
        });
      }
    }
    handleCloseModal();
  };

  return (
    <div className="container my-4">
      <h1 className="text-center mb-4">My Producers</h1>
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
          <ProducersTable producers={producers} onDelete={handleShowModal} />
        </div>
      </div>

      <DeleteModal
        show={showModal}
        itemId={selectedProducer?.id || null}
        itemName={selectedProducer?.name || ""}
        itemType="producer"
        onClose={handleCloseModal}
        onConfirm={handleConfirmDelete}
      />
    </div>
  );
}
